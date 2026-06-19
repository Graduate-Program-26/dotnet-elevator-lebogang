using Microsoft.Extensions.Hosting;
using MediatR;
using Microsoft.Extensions.Options;
public class RenderLoopService : BackgroundService
{
    private readonly IMediator _mediator;
    private readonly ConsoleRenderer _renderer;
    private readonly InputHandler _inputHandler;
    private readonly IOptions<SimOptions> _options;
    private readonly InMemoryLogSink _logSink;
    private const int RenderIntervalMs = 200;

    public RenderLoopService(IMediator mediator, ConsoleRenderer renderer, InputHandler inputHandler, InMemoryLogSink logSink, IOptions<SimOptions> options)
    {
        _mediator = mediator;
        _renderer = renderer;
        _inputHandler = inputHandler;
        _logSink = logSink;
        _options = options;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.CursorVisible = false;
        Console.Clear();
        
        while (!stoppingToken.IsCancellationRequested)
        {

            await _inputHandler.PollKeystroke(stoppingToken);

            var status = await _mediator.Send(new GetSystemStateQuery(), stoppingToken);

            var logs = _logSink.GetRecent(4);
            _renderer.DrawFrame(status, logs, _inputHandler.CurrentInput);

            await Task.Delay(RenderIntervalMs, stoppingToken);
        }

        Console.CursorVisible = true;
    }
}