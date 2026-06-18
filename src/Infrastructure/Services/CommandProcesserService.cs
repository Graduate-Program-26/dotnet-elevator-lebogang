using System.Threading.Channels;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class CommandProcessorService : BackgroundService
{
    private readonly IMediator _mediator;
    private readonly ILogger<CommandProcessorService> _logger;
    private readonly Channel<IBaseRequest> _channel; 

    public CommandProcessorService(IMediator mediator, ILogger<CommandProcessorService> logger,Channel<IBaseRequest> channel)
    {
        _mediator = mediator;
        _logger = logger;
        _channel = channel;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       _logger.LogInformation("Command processiong service started");

        await foreach(var request in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            await ProcessRequest(request, stoppingToken);
        }

        _logger.LogInformation("Command processiong service stopeped");
    }

    private async Task ProcessRequest(IBaseRequest request, CancellationToken token)
    {
        try
        {
            _logger.LogDebug("Processing {Request}", request.GetType().Name);

            await _mediator.Send(request, token);
        }
        catch (CommandException ex)
        {
            _logger.LogError(ex,"Error processing {RequestType}.",request.GetType().Name);
        }
    }
}