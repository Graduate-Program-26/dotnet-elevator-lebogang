using System.Threading.Channels;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrustractureServices(this IServiceCollection services, IConfiguration configuration)
    {
       services.Configure<SimOptions>(configuration.GetSection(SimOptions.SectionName));

        services.AddSingleton<Channel<IBaseRequest>>(Channel.CreateBounded<IBaseRequest>( new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,   // CommandProcessorService is the only reader
            SingleWriter = false   // multiple producers write to it
        }));
        
        services.AddSingleton<Building>();
        services.AddSingleton<IElevatorRepo, InMemoryElevatorRepo>();
        services.AddSingleton<IFloorRepo, InMemoryFloorRepo>();


        services.AddHostedService<CommandProcessorService>();
        services.AddHostedService<SimBackgroundService>();
        services.AddHostedService<PassengerGeneratorService>();
        services.AddHostedService<RenderLoopService>(); 

        services.AddSingleton<CommandParser>();
        services.AddSingleton<ConsoleRenderer>();
        services.AddSingleton<InputHandler>();

        
        return services;
    }
}