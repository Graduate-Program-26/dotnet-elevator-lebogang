using FluentAssertions;
using Moq;

public class QueriesTest
{
    private readonly PassengerElevatorFactory _elevatorFactory = new PassengerElevatorFactory();
    private Floor CreateFloor(int floorNumber = 1) => new Floor(floorNumber);

    [Fact]
    public async Task Handle_GetSystemStatus()
    {
        var elevatorOne = _elevatorFactory.CreateElevator(1);
        var elevatorTwo = _elevatorFactory.CreateElevator(5);

        var mockElevatorRepo = new Mock<IElevatorRepo>();
        mockElevatorRepo.Setup(r => r.GetElevators()).Returns(new List<IElevator> { elevatorOne, elevatorTwo });

        var mockFloorRepo = new Mock<IFloorRepo>();
        mockFloorRepo.Setup(r => r.GetFloors()).Returns(new List<Floor>());

        var handler = new GetSystemStateQueryHandler(mockElevatorRepo.Object, mockFloorRepo.Object);

        var result = await handler.Handle(new GetSystemStateQuery(), CancellationToken.None);

        result.Elevators.Should().HaveCount(2);
    }


    [Fact]
    public async Task Handle_MapsElevatorCurrentFloorCorrectly()
    {
        var elevator = _elevatorFactory.CreateElevator(startingFloor: 7);

        var mockElevatorRepo = new Mock<IElevatorRepo>();
        mockElevatorRepo.Setup(r => r.GetElevators())
            .Returns(new List<IElevator> { elevator });

        var mockFloorRepo = new Mock<IFloorRepo>();
        mockFloorRepo.Setup(r => r.GetFloors()).Returns(new List<Floor>());

        var handler = new GetSystemStateQueryHandler(
            mockElevatorRepo.Object, mockFloorRepo.Object);

        var result = await handler.Handle(
            new GetSystemStateQuery(), CancellationToken.None);

        result.Elevators[0].CurrentFloor.Should().Be(7);
    }

    [Fact]
    public async Task Handle_ReturnsWaitingPassengerCountPerFloor()
    {
        var floor = CreateFloor(3);
        floor.AddWaitingPassenger(new Passenger(sourceFloor: 3, destinationFloor: 7));
        floor.AddWaitingPassenger(new Passenger(sourceFloor: 3, destinationFloor: 9));

        var mockElevatorRepo = new Mock<IElevatorRepo>();
        mockElevatorRepo.Setup(r => r.GetElevators()).Returns(new List<IElevator>());

        var mockFloorRepo = new Mock<IFloorRepo>();
        mockFloorRepo.Setup(r => r.GetFloors()).Returns(new List<Floor> { floor });

        var handler = new GetSystemStateQueryHandler(mockElevatorRepo.Object, mockFloorRepo.Object);

        var result = await handler.Handle(new GetSystemStateQuery(), CancellationToken.None);

        result.Floors[0].WaitingPassengerCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_GetElevatorStatusById()
    {
        var elevator = _elevatorFactory.CreateElevator();

        var mockElevatorRepo = new Mock<IElevatorRepo>();
        mockElevatorRepo.Setup(r => r.GetElevator(elevator.Id)).Returns(elevator);

        var handler = new GetElevatorStateQueryHandler(mockElevatorRepo.Object);

        var result = await handler.Handle(new GetElevatorStateQuery(elevator.Id), CancellationToken.None);

        result.Id.Should().Be(elevator.Id);
    }


    [Fact]
    public async Task  Handle_InvalidId_ThrowsInvalidElevatorIdException()
    {
        var mockRepo = new Mock<IElevatorRepo>();
        mockRepo.Setup(r => r.GetElevator(It.IsAny<Guid>())).Returns((IElevator?)null);

        var handler = new GetElevatorStateQueryHandler(mockRepo.Object);

        Func<Task> act = () => handler.Handle(new GetElevatorStateQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidElevatorIdException>();   
    }

    [Fact]
    public async Task Handle_ElevatorWithPassengersAboard_MapsDestinations()
    {
        var elevator = _elevatorFactory.CreateElevator(startingFloor: 1);
        var passengers = new List<Passenger>
        {
            new Passenger(sourceFloor: 1, destinationFloor: 5),
            new Passenger(sourceFloor: 1, destinationFloor: 8)
        };
        elevator.BoardPassengers(passengers);

        var mockRepo = new Mock<IElevatorRepo>();
        mockRepo.Setup(r => r.GetElevator(elevator.Id)).Returns(elevator);

        var handler = new GetElevatorStateQueryHandler(mockRepo.Object);

        var result = await handler.Handle(
            new GetElevatorStateQuery(elevator.Id), CancellationToken.None);

        result.OnboardPassengerDestinations.Should().BeEquivalentTo(new[] { 5, 8 });
    }
}