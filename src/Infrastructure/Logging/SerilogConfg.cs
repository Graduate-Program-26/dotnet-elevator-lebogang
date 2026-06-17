using Serilog;

/// <summary>
/// Configures Serilog as the logging provider.
/// Called from Program.cs before the host is built.
/// Infrastructure owns this — Application never references Serilog directly.
/// </summary>
public static class SerilogConfiguration
{
    public static void Configure()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(
                outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/elevator-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }
}