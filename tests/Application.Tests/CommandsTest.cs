using FluentAssertions;
using Moq;


public class CommandTests
{
    private  PassengerElevatorFactory _elevatorFactory = new PassengerElevatorFactory();
    private Passenger CreatePassenger(int source = 0, int destination = 5) => new Passenger(source, destination);

    private static Floor CreateFloor(int floorNumber = 1)  => new Floor(floorNumber);
    private readonly MoveElevatorCommandHandler _moveElevatorHandler = new();
    private readonly BoardPassengersCommandHandler _boardPassengersdHandler = new();
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
        mockElevatorStrategy.Setup(strategy => strategy.FindBestElevator(It.IsAny<List<IElevator>>(), currentFloor, ElevatorDirection.Stationary)).Returns(elevator);

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


        await _disambarkPassengersHandler.Handle(new DisambarkPassengersCommand(currentFloor, elevator),CancellationToken.None);

        elevator.OnboardPassengers.Should().NotContain(passenger);

        elevator.CurrentCapacity.Should().Be(0);

        passenger.Status.Should().Be(PassengerStatus.Arrived);        
    }

    [Fact]
    public async Task Handle_BoardPassenegers_FromFloor() // pick up passangers
    {
        var elevator = _elevatorFactory.CreateElevator(1);
        const int currentFloor = 3;
        var floor = CreateFloor(currentFloor);
       
        var passaenger = CreatePassenger(1, 3);
        List<Passenger> passengers = new List<Passenger> {passaenger};

        await _boardPassengersdHandler.Handle(new BoardPassengersCommand(passengers, elevator), CancellationToken.None);

        elevator.FloorRequests.Should().Contain(currentFloor);
        elevator.OnboardPassengers.Should().Contain(passaenger);
    }

   
}