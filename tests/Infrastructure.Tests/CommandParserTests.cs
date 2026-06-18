using FluentAssertions;
using MediatR;

public class CommandParserTest
{
    private readonly CommandParser _commandParser = new();

    [Fact]
    public void Parse_Empty_ReturnsFailureMessage()
    {
        var result = _commandParser.Parse("");

        result.Should().BeOfType<ParseResult<IBaseRequest>.Failure>();
    }

    [Fact]
    public void Parse_UnknownVerb_ReturnsFailure()
    {
       var result = _commandParser.Parse("move 4");

       result.Should().BeOfType<ParseResult<IBaseRequest>.Failure>();
    }

    [Fact]
    public void Parse_CallCommandSameFloorAndDestination_ReturnsFailure()
    {
        var result = _commandParser.Parse("call 3 up 3");

        result.Should().BeOfType<ParseResult<IBaseRequest>.Failure>().Which.ErrorMessage.Should().Contain("differ");
    }

    [Fact]
    public void Parse_CallCommandDirectionDown_ParsesCorrectly()
    {
        var result = _commandParser.Parse("call 8 down 2");

        var command = ((ParseResult<IBaseRequest>.Success)result).Value.Should().BeOfType<RequestElevatorCommand>().Subject;

        command.Direction.Should().Be(ElevatorDirection.Down);
    }

    [Fact]
    public void Parse_SimOn_ReturnsToggleCommandEnabled()
    {
        var result = _commandParser.Parse("sim on");

        var command = ((ParseResult<IBaseRequest>.Success)result).Value.Should().BeOfType<ToggleSimCommand>().Subject;

        command.Enabled.Should().BeTrue();
    }

    [Fact]
    public void Parse_SimOff_ReturnsToggleCommandDisabled()
    {
        var result = _commandParser.Parse("sim off");

        var command = ((ParseResult<IBaseRequest>.Success)result).Value.Should().BeOfType<ToggleSimCommand>().Subject;

        command.Enabled.Should().BeFalse();
    }

    [Fact]
    public void Parse_StatusWithValidGuid_ReturnsGetElevatorStateQuery()
    {
        var id = Guid.NewGuid();
        var result = _commandParser.Parse($"status {id}");

        var command = ((ParseResult<IBaseRequest>.Success)result).Value.Should().BeOfType<GetElevatorStateQuery>().Subject;

        command.Id.Should().Be(id);
    }

    [Fact]
    public void Parse_Quit_ActuallyQuits()
    {
        var result = _commandParser.Parse("quit");

        ((ParseResult<IBaseRequest>.Success)result).Value.Should().BeOfType<Quit>();
    }
}