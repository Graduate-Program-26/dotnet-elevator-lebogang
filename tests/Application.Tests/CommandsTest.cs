using FluentAssertions;
using Moq;


public class CommandTests
{
    private  PassengerElevatorFactory _elevatorFactory = new PassengerElevatorFactory();
    private Passenger CreatePassenger(int source = 0, int destination = 5) => new Passenger(source, destination);

    private static Floor CreateFloor(int floorNumber = 1)  => new Floor(floorNumber);
    private readonly MoveElevatorCommandHandler _moveElevatorHandler = new();
    private readonly DisambarkPassengersCommandHandler _disambarkPassengersHandler = new();

    [Fact]
    public async Task Handle_WhenElevatorAvailable_AssignsFloorRequest() 
    {
        var elevator = _elevatorFactory.CreateElevator(1);
        const int currentFloor = 3;
        var floor = CreateFloor(currentFloor);

        var mockFloorRepo = new Mock<IFloorRepo>();
        mockFloorRepo.Setup(repo => repo.GetFloor(3)).Returns(floor);

        var mockElevatorRepo = new Mock<IElevatorRepo>();
        mockElevatorRepo.Setup(repo => repo.GetElevators()).Returns(new List<IElevator> {elevator});


        var mockElevatorStrategy = new Mock<IDispatchController>();
        mockElevatorStrategy
            .Setup(s => s.FindBestElevator(
                It.IsAny<List<IElevator>>(),
                It.IsAny<int>(),
                It.IsAny<ElevatorDirection>()))
            .Returns(elevator);

        var handler = new RequestElevatorCommandHandler(mockElevatorRepo.Object, mockFloorRepo.Object, mockElevatorStrategy.Object);
        
        // cancellation behavour is out of scope, forcing the test to run to completion, without it , the tests mught abort without hitting the assertion
        await handler.Handle(new RequestElevatorCommand(3, ElevatorDirection.Up), CancellationToken.None);

        elevator.FloorRequests.Should().Contain(3);
    }

    [Fact]
    public async Task Handle_WhenAssigningPassangersToElevator() // should move elevator to floor
    {
        var elevator = _elevatorFactory.CreateElevator(1);
        const int currentFloor = 3;
        var floor = CreateFloor(currentFloor);

        elevator.AddFloorRequest(currentFloor);

        await _moveElevatorHandler.Handle(new MoveElevatorCommand(elevator), CancellationToken.None);

        elevator.CurrentFloor.Should().Be(2);
    }

     [Fact]
    public async Task Handle_MultipleTicks_ReachesDestinationGradually()
    {
        var elevator = _elevatorFactory.CreateElevator(startingFloor: 1);
        int targetFloor = 4;
        elevator.AddFloorRequest(targetFloor);

        await _moveElevatorHandler.Handle(new MoveElevatorCommand(elevator), CancellationToken.None);
        elevator.CurrentFloor.Should().Be(2);

        await _moveElevatorHandler.Handle(new MoveElevatorCommand(elevator), CancellationToken.None);
        elevator.CurrentFloor.Should().Be(3);

        await _moveElevatorHandler.Handle(new MoveElevatorCommand(elevator), CancellationToken.None);
        elevator.CurrentFloor.Should().Be(4);
    }

    [Fact]
    public async Task Handle_NoFloorRequests_StateIsIdle()
    {
        var elevator = _elevatorFactory.CreateElevator(startingFloor: 3);

        await _moveElevatorHandler.Handle(new MoveElevatorCommand(elevator), CancellationToken.None);

        elevator.State.Should().Be(ElevatorState.Idle);
    }

    [Fact]
    public async Task Handle_WhenMoreRequestsRemain_StateStaysTraveling()
    {
        var elevator = _elevatorFactory.CreateElevator(startingFloor: 1);
        elevator.AddFloorRequest(3);
        elevator.AddFloorRequest(7);

        await _moveElevatorHandler.Handle(new MoveElevatorCommand(elevator), CancellationToken.None);

        // arrived at neither 3 nor 7 yet — still travelling
        elevator.State.Should().Be(ElevatorState.Traveling);
    }


    [Fact]
    public async Task Handle_DisembarkPassenegers_FromElevator() // remove passangers at their destinations
    {
        var elevator = _elevatorFactory.CreateElevator(startingFloor: 3);
        const int currentFloor = 3;
        var floor = CreateFloor(currentFloor);

        var passenger = CreatePassenger(source: 1, destination: 3);

        elevator.BoardPassengers(new List<Passenger> { passenger });

        await _disambarkPassengersHandler.Handle(new DisambarkPassengersCommand(currentFloor, elevator),CancellationToken.None);

        elevator.OnboardPassengers.Should().NotContain(passenger);

        elevator.CurrentCapacity.Should().Be(0);

        passenger.Status.Should().Be(PassengerStatus.Arrived);        
    }

    [Fact]
    public async Task Handle_BoardPassengers_FromFloor()
    {
        var elevator = _elevatorFactory.CreateElevator(startingFloor: 3);
        const int floorNumber = 3;
        var floor = CreateFloor(floorNumber);

        var passenger = CreatePassenger(source: 3, destination: 7);
        floor.AddWaitingPassenger(passenger);

        var mockFloorRepo = new Mock<IFloorRepo>();
        mockFloorRepo.Setup(r => r.GetFloor(floorNumber)).Returns(floor);

        var handler = new BoardPassengersCommandHandler(mockFloorRepo.Object);

        await handler.Handle(new BoardPassengersCommand(new List<Passenger> { passenger },elevator),CancellationToken.None);

        elevator.CurrentCapacity.Should().Be(1);

        elevator.FloorRequests.Should().Contain(7);

        passenger.Status.Should().Be(PassengerStatus.InTransit);

        floor.WaitingPassengers.Should().NotContain(passenger);
    }

    [Fact]
    public async Task Handle_WhenElevatorFull_NoPassengersBoarded()
    {
        var elevator = _elevatorFactory.CreateElevator(startingFloor: 3);

        // fill the elevator to capacity
        var existingPassengers = Enumerable.Range(0, elevator.MaxCapacity).Select(_ => CreatePassenger(source: 1, destination: 8)).ToList();
        elevator.BoardPassengers(existingPassengers);

        var waitingPassenger = CreatePassenger(source: 3, destination: 7);
        var floor = CreateFloor(3);
        floor.AddWaitingPassenger(waitingPassenger);

        var mockFloorRepo = new Mock<IFloorRepo>();
        mockFloorRepo.Setup(r => r.GetFloor(3)).Returns(floor);

        var handler = new BoardPassengersCommandHandler(mockFloorRepo.Object);

        await handler.Handle(new BoardPassengersCommand(new List<Passenger> { waitingPassenger },elevator),CancellationToken.None);


        floor.WaitingPassengers.Should().Contain(waitingPassenger);
        waitingPassenger.Status.Should().Be(PassengerStatus.Waiting);
    }

    [Fact]
    public async Task Handle_PartialBoarding_OnlyFitsAvailableSpace()
    {
        var elevator = _elevatorFactory.CreateElevator(startingFloor: 3);

        // board enough to leave only 1 space
        var existing = Enumerable.Range(0, elevator.MaxCapacity - 1).Select(_ => CreatePassenger(source: 1, destination: 8)).ToList();
        elevator.BoardPassengers(existing);

   
        var passengerA = CreatePassenger(source: 3, destination: 5);
        var passengerB = CreatePassenger(source: 3, destination: 6);
        var floor = CreateFloor(3);
        floor.AddWaitingPassenger(passengerA);
        floor.AddWaitingPassenger(passengerB);

        var mockFloorRepo = new Mock<IFloorRepo>();
        mockFloorRepo.Setup(r => r.GetFloor(3)).Returns(floor);

        var handler = new BoardPassengersCommandHandler(mockFloorRepo.Object);

        await handler.Handle(new BoardPassengersCommand(new List<Passenger> { passengerA, passengerB },elevator),CancellationToken.None);

        elevator.CurrentCapacity.Should().Be(elevator.MaxCapacity);

        floor.WaitingPassengers.Should().HaveCount(1);
    }

   
}