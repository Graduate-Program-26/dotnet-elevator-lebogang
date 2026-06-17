using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

public class MediatRTests
{
    private readonly MediatRPipeline _mediatRpipeline = new();
    private PassengerElevatorFactory _passengerElevatorFactory = new PassengerElevatorFactory();

    [Fact]
    public async Task Send_RequestElevatorCommand_RoutesToCorrectHandler()
    {
        var provider =  _mediatRpipeline.BuildProvider();
        var mediator = _mediatRpipeline.GetMediator(provider);


        Func<Task> act = () => mediator.Send(new RequestElevatorCommand(3, ElevatorDirection.Up));

        await act.Should().NotThrowAsync();
        
    }

    [Fact]
    public async Task Send_GetSystemStatusQuery_RoutesToCorrectHandler()
    {
        var provider =  _mediatRpipeline.BuildProvider();
        var mediator = _mediatRpipeline.GetMediator(provider);

        var result = await mediator.Send(new GetSystemStateQuery());

        result.Should().NotBeNull();
        result.Should().BeOfType<SystemStatusDto>();
    }

    [Fact]
    public async Task Send_GetElevatorStateQuery_RoutesToCorrectHandler()
    {
        var elevator = _passengerElevatorFactory.CreateElevator(2);

        var provider = _mediatRpipeline.BuildProvider(services =>
        {
            var mockRepo = new Mock<IElevatorRepo>();
            mockRepo.Setup(repo => repo.GetElevator(elevator.Id)).Returns(elevator);
            services.AddSingleton(mockRepo.Object);
        });

        var mediator = _mediatRpipeline.GetMediator(provider);

        var result = await mediator.Send(new GetElevatorStateQuery(elevator.Id));

        result.Should().NotBeNull();
        result.Id.Should().Be(elevator.Id);
        result.CurrentFloor.Should().Be(2);
    }

    [Fact]
    public async Task Send_InvalidCommand_ValidationBehaviourShortCircuits()
    {
        var provider =  _mediatRpipeline.BuildProvider();
        var mediator = _mediatRpipeline.GetMediator(provider);

        Func<Task> act = () => mediator.Send(new RequestElevatorCommand(-1, ElevatorDirection.Up));

        await act.Should().ThrowAsync<InvalidElevatorRequestException>();
    }

    [Fact]
    public async Task Send_InvalidCommand_HandlerNeverRuns()
    {
        var mockDispatch = new Mock<IDispatchController>();

        var provider = _mediatRpipeline.BuildProvider(services => services.AddSingleton(mockDispatch.Object));

        var mediator = _mediatRpipeline.GetMediator(provider);

        try
        {
            await mediator.Send(new RequestElevatorCommand(-1, ElevatorDirection.Up));
        }
        catch (InvalidElevatorRequestException)
        {
            
        }

        mockDispatch.Verify(dispatch => dispatch.FindBestElevator(It.IsAny<List<IElevator>>(),It.IsAny<int>(),It.IsAny<ElevatorDirection>()),Times.Never);
    }
}