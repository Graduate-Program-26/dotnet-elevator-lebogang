using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        /// using reflection, wire up all the IrequestHandlers
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
      
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

            // then add pipeline behaviours 
        });

        // wire up validators
        services.AddSingleton<IDispatchController, NearestElevatorStrategy>();

        return services;
    }
}