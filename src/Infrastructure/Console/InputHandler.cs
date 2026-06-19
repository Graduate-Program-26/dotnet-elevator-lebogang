using System.Text;
using System.Threading.Channels;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class InputHandler
{
    private readonly CommandParser _commandParser;
    private readonly ILogger<InputHandler> _logger;
    private readonly Channel<IBaseRequest> _channel;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly StringBuilder _stringBuilder = new();




    public InputHandler(Channel<IBaseRequest> channel, CommandParser commandParser, IHostApplicationLifetime lifetime, ILogger<InputHandler> logger)
    {
        _channel = channel;
        _commandParser = commandParser;
        _lifetime = lifetime;
        _logger = logger;
    }

    public string CurrentInput => _stringBuilder.ToString();

    public async Task PollKeystroke(CancellationToken token)
    {
        while(Console.KeyAvailable && !token.IsCancellationRequested)
        {
            var key = Console.ReadKey(intercept: true);
            await HandleKey(key, token);
        }
    }

    public async Task HandleKey(ConsoleKeyInfo key, CancellationToken token)
    {
        switch (key.Key)
        {
            case ConsoleKey.Enter:
                await SubmitToParser(token);
                break;

            case ConsoleKey.Backspace:
                if (_stringBuilder.Length > 0) _stringBuilder.Remove(_stringBuilder.Length - 1, 1);
                break;

            case ConsoleKey.Escape:
                _stringBuilder.Clear();
                break;

            default:
                // only append printable characters
                if (!char.IsControl(key.KeyChar)) _stringBuilder.Append(key.KeyChar);
                break;
        }
        
    }

    private async Task SubmitToParser(CancellationToken token)
    {
        var input = _stringBuilder.ToString().Trim();
        _stringBuilder.Clear();

        if(string.IsNullOrEmpty(input)) return;

        _logger.LogDebug("User inout: '{input}' ", input);

        var result = _commandParser.Parse(input);

        switch (result)
        {
            case ParseResult<IBaseRequest[]>.Success success:
                foreach (var request in success.Value)
                    await HandleSuccess(request, token);
                break;

            case ParseResult<IBaseRequest[]>.Failure f:
                RenderError(f.ErrorMessage);
                break;
        }
    }

    private async  Task HandleSuccess(IBaseRequest request, CancellationToken token)
    {
        switch(request)
        {
            case Quit:
                _logger.LogInformation("Quit requested by user.");
                _lifetime.StopApplication();
                break;

        // everything else goes through the channel
            default:
                await _channel.Writer.WriteAsync(request, token);
                break;
        }
    }

    private static void RenderError(string errorMessage)
    {
        var original = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  Error: {errorMessage}");
        Console.ForegroundColor = original;
    }

}