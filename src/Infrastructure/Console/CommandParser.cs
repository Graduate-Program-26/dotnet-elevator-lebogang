using MediatR;

public class CommandParser
{
    // dictionary isn't storing raw data; it is storing a collection of executable functions.
    private readonly Dictionary<string, Func<string[], ParseResult<IBaseRequest[]>>> _commandRegistry;

    public CommandParser()
    {
        _commandRegistry = new Dictionary<string, Func<string[], ParseResult<IBaseRequest[]>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["call"] = ParseCallElevator,
            ["status"] = ParseSystemStatus,
            ["sim"] = ParseSimToggle,
            ["quit"] = ParseQuit
        };
    }

    public ParseResult<IBaseRequest[]> Parse(string input)
    {
        if(string.IsNullOrWhiteSpace(input))
        {
            return Fail("No command entered");
        }

        var commandTokens =  input.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var action = commandTokens[0];

        if(!_commandRegistry.TryGetValue(action, out var factory))
        {
            return Fail($"Unknown command '{action}' ");
        }

        // a factory is an object or method responsible for creating other objects.
        //  However,  the factory is a Functional Delegate Factory.
        return factory(commandTokens);
    }


    // mapping string commands that are parsed to the rest of the infra
    

    private static ParseResult<IBaseRequest[]> ParseCallElevator(string[] commandTokens)
    {
        if (commandTokens.Length != 4)
            return Fail("Usage: call [floor] [up|down] [destination]");

        if (!int.TryParse(commandTokens[1], out var floor))
            return Fail($"Floor '{commandTokens[1]}' is not a valid number.");

        if (!TryParseDirection(commandTokens[2], out var direction))
            return Fail($"Direction '{commandTokens[2]}' must be 'up' or 'down'.");

        if (!int.TryParse(commandTokens[3], out var destination))
            return Fail($"Destination '{commandTokens[3]}' is not a valid number.");

        if (floor < 0)
            return Fail("Floor number must be 0 or above.");

        if (destination < 0)
            return Fail("Destination must be 0 or above.");

        if (destination == floor)
            return Fail("Destination must differ from the current floor.");

        return new ParseResult<IBaseRequest[]>.Success(new IBaseRequest[]
        {
            new RegisterPassengerCommand(floor, destination),
            new RequestElevatorCommand(floor, direction)
        });
    }

    private static ParseResult<IBaseRequest[]> ParseSystemStatus(string[] commandTokens)
    {
        if(commandTokens.Length == 1) return Single(new GetSystemStateQuery());

        if(!Guid.TryParse(commandTokens[1], out var elevatorId)) return Fail($" '{commandTokens}' is not a valid elevatorId ");

        return Single(new GetElevatorStateQuery(elevatorId));

    }

    private static ParseResult<IBaseRequest[]> ParseSimToggle(string[] commandTokens)
    {
        if (commandTokens.Length != 2) return Fail("Usage: sim [on|off]");

        return commandTokens[1].ToLowerInvariant() switch
            {
                "on"  => Single(new ToggleSimCommand(Enabled: true)),
                "off" => Single(new ToggleSimCommand(Enabled: false)),
                _     => Fail($"'{commandTokens[1]}' must be 'on' or 'off'.")
            };
    }


    private static ParseResult<IBaseRequest[]> ParseQuit(string[] commandTokens)
    {
        return Single(new Quit());
    }

    private static ParseResult<IBaseRequest[]>.Success Single(IBaseRequest request)
        => new(new IBaseRequest[] { request });

    private static ParseResult<IBaseRequest[]>.Failure Fail(string message)
        => new(message);

    private static bool TryParseDirection(string token, out ElevatorDirection direction)
    {
        switch (token.ToLowerInvariant())
        {
            case "up":
                direction = ElevatorDirection.Up;
                return true;
            case "down":
                direction = ElevatorDirection.Down;
                return true;
            default:
                direction = ElevatorDirection.Stationary;
                return false;
        }
    }

}