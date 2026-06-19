using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("ElevatorOS starting up.");
    var memorySink = new InMemoryLogSink();

    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog((context, services, loggerConfig) =>
        {
             loggerConfig
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", 
                    Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System",    
                    Serilog.Events.LogEventLevel.Warning)
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] " +
                    "{Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/elevator-.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] " +
                    "{Message:lj}{NewLine}{Exception}")
                .WriteTo.Sink(memorySink);
        })
        .ConfigureServices((context, services) =>
        {
            services.AddSingleton(memorySink);
            services.AddLogging();
            services.AddApplicationServices();
            services.AddInfrustractureServices(context.Configuration);
        })
        .Build();


    await SeedBuilding(host.Services);

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ElevatorOS terminated unexpectedly.");
}
finally
{
    await Log.CloseAndFlushAsync();
}


/// <summary>
/// Initialises the building and seeds repositories from it.
/// Called once after the host is built but before it starts.
/// This is the only place that constructs concrete elevator types.
/// </summary>
static async Task SeedBuilding(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var sp = scope.ServiceProvider;

    var options = sp.GetRequiredService<IOptions<SimOptions>>().Value;
    var logger  = sp.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("Initialising {BuildingName} with {Floors} floors and {Elevators} elevators",options.TotalFloors,options.TotalElevators);


    var elevators = BuildElevators(options.TotalElevators);

    var building = sp.GetRequiredService<Building>();
    building.Initialise(options.TotalFloors, elevators);

    // seed repositories from building's authoritative lists
    // repos hold references to the same objects — mutations
    // made by command handlers are immediately visible to queries
    var elevatorRepo = sp.GetRequiredService<IElevatorRepo>() as InMemoryElevatorRepo;
    var floorRepo = sp.GetRequiredService<IFloorRepo>() as InMemoryFloorRepo;

    elevatorRepo!.Seed(building.Elevators);
    floorRepo!.Seed(building.Floors);

    logger.LogInformation("Building seeded successfully.");

    await Task.CompletedTask;
}

static List<IElevator> BuildElevators(int count)
{
    var elevators = new List<IElevator>();
    var passengerElevatorFactory = new PassengerElevatorFactory();
    var glassElevatorFactory = new GlassElevatorFactory();
    var highSpeedElavatorFactory = new HighSpeedElavatorFactory();
    var serviceElavatorFactory = new ServiceElevatorFactory();


    for (int i = 0; i < count; i++)
    {
        // distribute elevator types across the building
        IElevator elevator = (i % 4) switch
        {
            0 => passengerElevatorFactory.CreateElevator(0),
            1 => glassElevatorFactory.CreateElevator(0),
            2 => serviceElavatorFactory.CreateElevator(0),
            _ => highSpeedElavatorFactory.CreateElevator(0)
        };

        elevators.Add(elevator);
    }

    return elevators;
}