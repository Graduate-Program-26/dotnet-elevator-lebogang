using FluentAssertions;
using Moq;

public class QueriesTest
{
    private readonly PassengerElevatorFactory _elevatorFactory = new PassengerElevatorFactory();
    private Floor CreateFloor(int floorNumber = 1) => new Floor(floorNumber);

    [Fact]
    public async Task Handel_GetSystemStatus()
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

}