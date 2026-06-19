
using MediatR;
using Microsoft.Extensions.Logging;
/// <summary>
/// Pipeline behaviour that logs every incoming request and its outcome.
/// Runs before the handler for every command and query automatically.
/// </summary>
public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling {RequestName}", requestName);

        var response = await next(); // call the next behaviour or handler

        _logger.LogInformation("Handled {RequestName}", requestName);

        return response;
    }
}