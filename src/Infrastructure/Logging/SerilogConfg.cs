using Serilog;


public static class SerilogConfiguration
{
    public static void Configure()
    {
        var sink = new InMemoryLogSink();
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
            .WriteTo.Sink(sink)
            .CreateLogger();
    }
}