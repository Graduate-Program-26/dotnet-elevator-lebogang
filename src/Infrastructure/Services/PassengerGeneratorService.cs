using System.Threading.Channels;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class PassengerGeneratorService : BackgroundService
{
    private readonly Channel<IBaseRequest> _channel;
    private readonly IFloorRepo _floorRepo;
    private readonly IOptions<SimOptions> _options;
    private readonly ILogger<PassengerGeneratorService> _logger;

    private readonly Random _random = new();

    // toggled by ToggleSimulationCommand via a thread-safe flag (volatile)
    private volatile bool _enabled;

    public PassengerGeneratorService(Channel<IBaseRequest> channel,IFloorRepo floorRepo,IOptions<SimOptions> options,ILogger<PassengerGeneratorService> logger)
    {
        _channel  = channel;
        _floorRepo = floorRepo;
        _options  = options;
        _logger   = logger;
        _enabled  = options.Value.PassengerGenerationEngine.AutoGen;
    }

   
    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
        _logger.LogInformation("Passenger generation {Status}.",enabled ? "enabled" : "disabled");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var opts = _options.Value.PassengerGenerationEngine;

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_enabled)
            {
                try
                {
                    await GeneratePassengersAsync(stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error generating passengers.");
                }
            }

            await Task.Delay(opts.GenInterval, stoppingToken);
        }
    }

    /// <summary>
    /// Generates a random batch of passengers across random floors
    /// and writes RequestElevatorCommands to the channel.
    /// </summary>
    private async Task GeneratePassengersAsync(CancellationToken stoppingToken)
    {
        var opts   = _options.Value.PassengerGenerationEngine;
        var floors = _floorRepo.GetFloors().ToList();

        if (floors.Count < 2) return;

        // pick random subset of floors to receive passengers this tick
        var floorsToPopulate = floors.OrderBy(_ => _random.Next()).Take(_random.Next(1, opts.MaxFloorsPerTick + 1)).ToList();

        foreach (var curFloor in floorsToPopulate)
        {
            var passengerCount = _random.Next(1, opts.MaxPassengersPerFloor + 1);

            // pick a random destination on a different floor
            var validDestinations = floors.Where(floor => curFloor.FloorNumber != floor.FloorNumber).ToList();

            if (!validDestinations.Any()) continue;

            var destination = validDestinations[_random.Next(validDestinations.Count)].FloorNumber;

            var direction = destination > curFloor.FloorNumber ? ElevatorDirection.Up : ElevatorDirection.Down;

            _logger.LogDebug("Generating {Count} passenger(s) on floor {Floor} -> {Dest}.",passengerCount, curFloor.FloorNumber, destination);

            for (int i = 0; i < passengerCount; i++)
            {
                await _channel.Writer.WriteAsync(new RequestElevatorCommand(curFloor.FloorNumber, direction), stoppingToken);
            }
        }
    }
}