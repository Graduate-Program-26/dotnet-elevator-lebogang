using FluentAssertions;
using MediatR;

public class CommandParserTest
{
    private readonly CommandParser _commandParser = new();


    [Fact]
    public void Parse_Empty_ReturnsFailure()
    {
        var result = _commandParser.Parse("");

        // generic type is now IBaseRequest[] everywhere
        result.Should().BeOfType<ParseResult<IBaseRequest[]>.Failure>();
    }

    [Fact]
    public void Parse_UnknownVerb_ReturnsFailure()
    {
        var result = _commandParser.Parse("move 4");

        result.Should().BeOfType<ParseResult<IBaseRequest[]>.Failure>().Which.ErrorMessage.Should().Contain("move");
    }

    [Fact]
    public void Parse_CallCommandSameFloorAndDestination_ReturnsFailure()
    {
        var result = _commandParser.Parse("call 3 up 3");

        result.Should().BeOfType<ParseResult<IBaseRequest[]>.Failure>().Which.ErrorMessage.Should().Contain("differ");
    }

    [Fact]
    public void Parse_CallCommandMissingArgs_ReturnsFailure()
    {
        var result = _commandParser.Parse("call 3 up");

        result.Should().BeOfType<ParseResult<IBaseRequest[]>.Failure>().Which.ErrorMessage.Should().Contain("Usage");
    }

    [Fact]
    public void Parse_CallCommandInvalidDirection_ReturnsFailure()
    {
        var result = _commandParser.Parse("call 3 sideways 7");

        result.Should().BeOfType<ParseResult<IBaseRequest[]>.Failure>().Which.ErrorMessage.Should().Contain("sideways");
    }


    [Fact]
    public void Parse_CallCommand_ReturnsTwoRequests()
    {
        var result = _commandParser.Parse("call 3 up 7");

        var success = result.Should()
            .BeOfType<ParseResult<IBaseRequest[]>.Success>().Subject;

        // must be exactly two — register passenger then dispatch
        success.Value.Should().HaveCount(2);
    }

    [Fact]
    public void Parse_CallCommand_FirstRequestIsRegisterPassenger()
    {
        var result = _commandParser.Parse("call 3 up 7");

        var success = (ParseResult<IBaseRequest[]>.Success)result;

        // passenger must be registered BEFORE elevator is dispatched
        var register = success.Value[0].Should().BeOfType<RegisterPassengerCommand>().Subject;

        register.SourceFloor.Should().Be(3);
        register.DestinationFloor.Should().Be(7);
    }

    [Fact]
    public void Parse_CallCommand_SecondRequestIsRequestElevator()
    {
        var result = _commandParser.Parse("call 3 up 7");

        var success = (ParseResult<IBaseRequest[]>.Success)result;

        var dispatch = success.Value[1].Should().BeOfType<RequestElevatorCommand>().Subject;

        dispatch.FloorNumber.Should().Be(3);
        dispatch.Direction.Should().Be(ElevatorDirection.Up);
    }

    [Fact]
    public void Parse_CallCommandDirectionDown_ParsesCorrectly()
    {
        var result = _commandParser.Parse("call 8 down 2");

        var success = (ParseResult<IBaseRequest[]>.Success)result;

        // index [1] is the RequestElevatorCommand
        var dispatch = success.Value[1].Should().BeOfType<RequestElevatorCommand>().Subject;

        dispatch.Direction.Should().Be(ElevatorDirection.Down);
    }


    [Fact]
    public void Parse_SimOn_ReturnsToggleCommandEnabled()
    {
        var result = _commandParser.Parse("sim on");

        var success = (ParseResult<IBaseRequest[]>.Success)result;

        // single item array — index [0]
        var command = success.Value[0].Should()
            .BeOfType<ToggleSimCommand>().Subject;

        command.Enabled.Should().BeTrue();
    }

    [Fact]
    public void Parse_SimOff_ReturnsToggleCommandDisabled()
    {
        var result = _commandParser.Parse("sim off");

        var success = (ParseResult<IBaseRequest[]>.Success)result;

        var command = success.Value[0].Should().BeOfType<ToggleSimCommand>().Subject;

        command.Enabled.Should().BeFalse();
    }

    [Fact]
    public void Parse_SimInvalidArg_ReturnsFailure()
    {
        var result = _commandParser.Parse("sim maybe");

        result.Should().BeOfType<ParseResult<IBaseRequest[]>.Failure>();
    }

    // ── STATUS COMMAND ───────────────────────────────────────

    [Fact]
    public void Parse_StatusNoArgs_ReturnsGetSystemStatusQuery()
    {
        var result = _commandParser.Parse("status");

        var success = (ParseResult<IBaseRequest[]>.Success)result;

        success.Value[0].Should().BeOfType<GetSystemStateQuery>();
    }

    [Fact]
    public void Parse_StatusWithValidGuid_ReturnsGetElevatorStateQuery()
    {
        var id = Guid.NewGuid();
        var result = _commandParser.Parse($"status {id}");

        var success = (ParseResult<IBaseRequest[]>.Success)result;

        var query = success.Value[0].Should().BeOfType<GetElevatorStateQuery>().Subject;

        // verify the correct ID was parsed
        query.Id.Should().Be(id);
    }

    [Fact]
    public void Parse_StatusWithInvalidGuid_ReturnsFailure()
    {
        var result = _commandParser.Parse("status not-a-guid");

        result.Should().BeOfType<ParseResult<IBaseRequest[]>.Failure>();
    }



    [Fact]
    public void Parse_Quit_ReturnsQuitRequest()
    {
        var result = _commandParser.Parse("quit");

        var success = (ParseResult<IBaseRequest[]>.Success)result;

        success.Value[0].Should().BeOfType<Quit>();
    }


    [Theory]
    [InlineData("CALL 3 UP 7")]
    [InlineData("Call 3 Up 7")]
    [InlineData("call 3 up 7")]
    public void Parse_CallCommand_IsCaseInsensitive(string input)
    {
        var result = _commandParser.Parse(input);

        result.Should().BeOfType<ParseResult<IBaseRequest[]>.Success>();
    }


    [Theory]
    [InlineData("call 0 up 5",   0, ElevatorDirection.Up,   5)]
    [InlineData("call 9 down 1", 9, ElevatorDirection.Down,  1)]
    [InlineData("call 5 up 10",  5, ElevatorDirection.Up,   10)]
    public void Parse_CallCommand_VariousInputs_ParsedCorrectly(string input, int expectedFloor,ElevatorDirection expectedDirection, int expectedDest)
    {
        var result = _commandParser.Parse(input);
        var success = (ParseResult<IBaseRequest[]>.Success)result;

        // check passenger registration
        var register = (RegisterPassengerCommand)success.Value[0];
        register.SourceFloor.Should().Be(expectedFloor);
        register.DestinationFloor.Should().Be(expectedDest);

        // check elevator dispatch
        var dispatch = (RequestElevatorCommand)success.Value[1];
        dispatch.FloorNumber.Should().Be(expectedFloor);
        dispatch.Direction.Should().Be(expectedDirection);
    }
}