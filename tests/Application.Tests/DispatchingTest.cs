using FluentAssertions;

public class DispatchingTest
{
    private PassengerElevatorFactory _elevatorFactory = new PassengerElevatorFactory();
    private GlassElevatorFactory _glassElevatorFactory = new GlassElevatorFactory();
    private HighSpeedElavatorFactory _highSpeedElavatorFactory = new HighSpeedElavatorFactory();

    private ServiceElevatorFactory _serviceElevatorFactory = new ServiceElevatorFactory();


    private List<Passenger> CreatePassenger(int numbwerOfPasengers, int source = 0, int destination = 5)
    {
        List<Passenger> passengers = new();
        for (int i = 0; i < numbwerOfPasengers; i++)
        {
             passengers.Add( new Passenger(source, destination));
        }
      
        return passengers;
    } 


    [Fact]
    public void FindBestElevator_ReturnsClosestElevator()
    {
        var nearElevator = _elevatorFactory.CreateElevator(3);
        var farElevator = _glassElevatorFactory.CreateElevator(6);
        var reallyFarElevator = _serviceElevatorFactory.CreateElevator(9);

        var elevators = new List<IElevator> {nearElevator, farElevator, reallyFarElevator} ;
        var elevatorStrategy = new NearestElevatorStrategy();

        var bestElevator =  elevatorStrategy.FindBestElevator(elevators, currentFloor: 4, requestedDirection: ElevatorDirection.Up);

        bestElevator.Should().Be(nearElevator);
    }


    [Fact]
    public void FindBestElevator_NearestElevatorFull_ReturnsNextClosest()
    {
        var fullElevator = _elevatorFactory.CreateElevator(3);
        fullElevator.BoardPassengers(CreatePassenger(fullElevator.MaxCapacity)); 

        var availableElevator = _elevatorFactory.CreateElevator(6);

        var elevators = new List<IElevator> { fullElevator, availableElevator };
        var strategy = new NearestElevatorStrategy();

        var best = strategy.FindBestElevator(elevators, currentFloor: 4, requestedDirection: ElevatorDirection.Up);

        best.Should().Be(availableElevator);
    }

    [Fact]
    public void FindBestElevator_AllElevatorsFull_ReturnsNull()
    {
        var elevatorOne = _elevatorFactory.CreateElevator(3);
        elevatorOne.BoardPassengers(CreatePassenger(elevatorOne.MaxCapacity));

        var elevatorTwo = _elevatorFactory.CreateElevator(6);
        elevatorTwo.BoardPassengers(CreatePassenger(elevatorTwo.MaxCapacity));

        var elevators = new List<IElevator> { elevatorOne, elevatorTwo };
        var strategy = new NearestElevatorStrategy();

        var best = strategy.FindBestElevator(elevators, currentFloor: 4 , requestedDirection: ElevatorDirection.Up);

        best.Should().BeNull(); // passenger must wait still, keep retrying
    }

    [Fact]
    public void FindBestElevator_ElevatorInMaintenance_IsExcluded()
    {
        var maintenanceElevator = _elevatorFactory.CreateElevator(3);
        maintenanceElevator.State = ElevatorState.OutOfService;

        var availableElevator = _elevatorFactory.CreateElevator(8);

        var elevators = new List<IElevator> { maintenanceElevator, availableElevator };
        var strategy = new NearestElevatorStrategy();

        var best = strategy.FindBestElevator(elevators, currentFloor: 4, requestedDirection: ElevatorDirection.Up);

        best.Should().Be(availableElevator);
    }


    [Fact]
    public void FindBestElevator_ElevatorPassingFloorInOppositeDirection_IsNotPreferred()
    {
        // Passenger on floor 4 wants to go UP
        // Elevator at floor 1, heading toward floor 8 — will PASS floor 4 going UP
        var elevatorGoingUp = _elevatorFactory.CreateElevator(1);
        elevatorGoingUp.AddFloorRequest(8);

        // Elevator at floor 6, heading toward floor 1 — will PASS floor 4 going DOWN
        var elevatorGoingDown = _elevatorFactory.CreateElevator(6);
        elevatorGoingDown.AddFloorRequest(1);

        var elevators = new List<IElevator> { elevatorGoingDown, elevatorGoingUp };
        var strategy = new NearestElevatorStrategy();

        // Passenger wants UP — the elevator already heading UP should win,
        // even though elevatorGoingDown might be numerically closer at some point
        var best = strategy.FindBestElevator(elevators, currentFloor: 4, requestedDirection: ElevatorDirection.Up);

        best.Should().Be(elevatorGoingUp);
    }

    [Fact]
    public void FindBestElevator_PassengerWantsDown_ElevatorGoingUpNotPreferred()
    {
        // Passenger on floor 4 wants to go DOWN
        var elevatorGoingUp = _elevatorFactory.CreateElevator(2);
        elevatorGoingUp.AddFloorRequest(10); // passes floor 4 going UP

        var idleElevatorBelow = _elevatorFactory.CreateElevator(0);

        var elevators = new List<IElevator> { elevatorGoingUp, idleElevatorBelow };
        var strategy = new NearestElevatorStrategy();

        var best = strategy.FindBestElevator(elevators, currentFloor: 4, requestedDirection: ElevatorDirection.Down);

        // elevatorGoingUp is technically closer and "passes" floor 4,
        // but is heading the WRONG way for this passenger
        best.Should().NotBe(elevatorGoingUp);
    }

    [Fact]
    public void FindBestElevator_TwoElevatorsEquidistant_ReturnsConsistentChoice()
    {
        var elevatorA = _elevatorFactory.CreateElevator(2); // 2 floors away
        var elevatorB = _elevatorFactory.CreateElevator(6); // 2 floors away

        var elevators = new List<IElevator> { elevatorA, elevatorB };
        var strategy = new NearestElevatorStrategy();

        var best = strategy.FindBestElevator(elevators, currentFloor: 4, requestedDirection: ElevatorDirection.Up);

        // tie breaking must take direction into account, choose the elevator from the opisite direction from requested direction, because why not.
        best.Should().Be(elevatorA);
    }

    [Fact]
    public void FindBestElevator_ElevatorAlreadyAtRequestedFloor_IsChosen()
    {
        var elevatorAtFloor = _elevatorFactory.CreateElevator(4); // already here
        var elevatorNearby = _elevatorFactory.CreateElevator(5);

        var elevators = new List<IElevator> { elevatorNearby, elevatorAtFloor };
        var strategy = new NearestElevatorStrategy();

        var best = strategy.FindBestElevator(elevators, currentFloor: 4, requestedDirection: ElevatorDirection.Up);

        best.Should().Be(elevatorAtFloor);
    }

    [Fact]
    public void FindBestElevator_EmptyElevatorList_ReturnsNull()
    {
        var elevators = new List<IElevator>();
        var strategy = new NearestElevatorStrategy();

        var best = strategy.FindBestElevator(elevators, currentFloor: 4, requestedDirection: ElevatorDirection.Up);

        best.Should().BeNull();
    }


    [Fact]
    public void FindBestElevator_SingleElevator_ReturnsThatElevator()
    {
        var onlyElevator = _elevatorFactory.CreateElevator(7);
        var elevators = new List<IElevator> { onlyElevator };
        var strategy = new NearestElevatorStrategy();

        var best = strategy.FindBestElevator(elevators, currentFloor: 4, requestedDirection: ElevatorDirection.Up);

        best.Should().Be(onlyElevator);
    }

}