using System.Threading.Channels;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class SimBackgroundService : BackgroundService
{
    private readonly IElevatorRepo _elevatorRepo;
    private readonly IFloorRepo _floorRepo;
    private readonly ILogger<SimBackgroundService> _logger;
    private readonly Channel<IBaseRequest> _channel;

    private readonly IOptions<SimOptions> _options;

    public SimBackgroundService(IFloorRepo floorRepo, IElevatorRepo elevatorRepo, ILogger<SimBackgroundService> logger, Channel<IBaseRequest> channel, IOptions<SimOptions> options)
    {
        _elevatorRepo = elevatorRepo;
        _floorRepo = floorRepo;
        _logger = logger;
        _channel = channel;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       _logger.LogInformation("simualtion started");

        while(!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Tick(stoppingToken);
            }
           catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // log bad ticks but keep the simulation running
                _logger.LogError(ex, "Error during simulation tick.");
            }

            await Task.Delay(_options.Value.TickInterval, stoppingToken);
        }

        _logger.LogInformation("simualtion stopped");
    }

    private async Task Tick(CancellationToken token)
    {
        var elevators = _elevatorRepo.GetElevators();

        foreach (var elevator in elevators)
        {
            if(token.IsCancellationRequested) break;

            await ProcessElevatorTick(elevator, token);
        }
    }

    /// limuit visibility to current compiled project instead of private/
    internal async Task ProcessElevatorTick(IElevator elevator, CancellationToken token)
    {
        var floorRequests = elevator.FloorRequests.ToList();

        await _channel.Writer.WriteAsync(new MoveElevatorCommand(elevator) , token);

        // chack that eleabvtor has arrived will move elevator command excutes 

        await Task.Yield();

        bool arrivedAtFloor =  floorRequests.Contains(elevator.CurrentFloor) && !elevator.FloorRequests.Contains(elevator.CurrentFloor);

        if(!arrivedAtFloor) return;

        _logger.LogInformation("Elevator {id} arrived at floor {floorNumber}", elevator.Id, elevator.CurrentFloor);


        await _channel.Writer.WriteAsync(new DisambarkPassengersCommand(elevator.CurrentFloor, elevator), token);

        var floor = _floorRepo.GetFloor(elevator.CurrentFloor);

        if(floor.HasWaitingPassengers)
        {
            await _channel.Writer.WriteAsync(new BoardPassengersCommand(floor.WaitingPassengers.ToList(), elevator), token);
        }
    }
}