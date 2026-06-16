using FluentAssertions;

public class ElevatorTest
{
    private  PassengerElevatorFactory _elevatorFactory = new PassengerElevatorFactory();
    private Passenger CreatePassenger(int source = 0, int destination = 5) => new Passenger(source, destination);
    private static void BoardPassengers(IElevator elevator, params Passenger[] passengers) => elevator.BoardPassengers(passengers.ToList());

    private List<Passenger> CreatePassengers(int numbwerOfPasengers, int source = 0, int destination = 5)
    {
        List<Passenger> passengers = new();
        for (int i = 0; i < numbwerOfPasengers; i++)
        {
             passengers.Add( new Passenger(source, destination));
        }
      
        return passengers;
    } 

    [Fact]
    public void MoveOneFloor_WhenRequestFloorAbove_DirectionIsUp()
    {
        var elevator = _elevatorFactory.CreateElevator(); // starts on ground floor by default

        elevator.AddFloorRequest(5);

        elevator.RequestedDirection.Should().Be(ElevatorDirection.Up);
    }

    [Fact]
    public void MoveOneFloor_MovesElevatorOneFloorInCurrentDirection()
    {
        var elevator = _elevatorFactory.CreateElevator(1);
        elevator.AddFloorRequest(5);

        elevator.MoveOneFloor();

        elevator.CurrentFloor.Should().Be(2);
    }

    [Fact]
    public void MoveOneFloor_WhenNoRequests_DoesNotMove()
    {
        var elevator = _elevatorFactory.CreateElevator(3);

        elevator.MoveOneFloor();

        elevator.CurrentFloor.Should().Be(3);
    }

    [Fact]
    public void MoveOneFloor_WhenArrivesAtRequestedFloor_BecomesStationary()
    {
        var elevator = _elevatorFactory.CreateElevator(4);
        elevator.AddFloorRequest(5);

        elevator.MoveOneFloor(); // moves to 5

        elevator.RequestedDirection.Should().Be(ElevatorDirection.Stationary);
        elevator.State.Should().Be(ElevatorState.Idle);
    }



    [Fact]
    public void BoardPassengers_WhenWithinCapacity_IncreasesCount()
    {
        var elevator = _elevatorFactory.CreateElevator();

        elevator.BoardPassengers(CreatePassengers(3));

        elevator.CurrentCapacity.Should().Be(3);
    }

    [Fact]
    public void BoardPassengers_WhenExceedsCapacity_ThrowsException()
    {
        var elevator =_elevatorFactory.CreateElevator();
        elevator.BoardPassengers(CreatePassengers(elevator.MaxCapacity));

        // MaxCapacity is now full — boarding one more must throw
        Action act = () => elevator.BoardPassengers(1);

        act.Should().Throw<ElevatorCapacityException>().WithMessage("*capacity*");
    }

    [Fact]
    public void HasCapacity_WhenSpaceAvailable_ReturnsTrue()
    {
        var elevator = _elevatorFactory.CreateElevator();
        elevator.BoardPassengers(CreatePassengers(2));

        elevator.HasCapacity().Should().BeTrue();
    }


    [Fact]
    public void AddFloorRequest_DuplicateFloor_OnlyAddedOnce()
    {
        var elevator = _elevatorFactory.CreateElevator();

        elevator.AddFloorRequest(5);
        elevator.AddFloorRequest(5);

        elevator.FloorRequests.Should().HaveCount(1);
    }

    [Fact]
    public void DisembarkAtCurrentFloor_PassengerAtDestination_DecreaseCapacity()
    {
        var elevator = _elevatorFactory.CreateElevator(startingFloor: 3);
        var passenger = CreatePassenger(source: 1, destination: 3);
        BoardPassengers(elevator, passenger);

        elevator.DisembarkAtCurrentFloor();

        elevator.CurrentCapacity.Should().Be(0);
    }
   
    [Fact]
    public void DisembarkAtCurrentFloor_PassengerAtDestination_IsRemoved()
    {
        var elevator = _elevatorFactory.CreateElevator(startingFloor: 3);
        var passenger = CreatePassenger(source: 1, destination: 3);
        BoardPassengers(elevator, passenger);

        elevator.DisembarkAtCurrentFloor();

        elevator.OnboardPassengers.Should().NotContain(passenger);
    }

    [Fact]
    public void DisembarkAtCurrentFloor_OnlyMatchingDestination_Disembarks()
    {
        var elevator = _elevatorFactory.CreateElevator(startingFloor: 3);

        var arriving  = CreatePassenger(source: 1, destination: 3); // gets off here
        var continuing = CreatePassenger(source: 1, destination: 7); // stays on

        BoardPassengers(elevator, arriving, continuing);

        elevator.DisembarkAtCurrentFloor();

        elevator.OnboardPassengers.Should().NotContain(arriving);
        elevator.OnboardPassengers.Should().Contain(continuing);
        elevator.CurrentCapacity.Should().Be(1);
    }

    [Theory]
    [InlineData(0, 5, ElevatorDirection.Up)]
    [InlineData(8, 3, ElevatorDirection.Down)]
    [InlineData(5, 5, ElevatorDirection.Stationary)]
    public void AddFloorRequest_SetsCorrectDirection(
        int startFloor, int requestedFloor, ElevatorDirection expected)
    {
        var elevator =_elevatorFactory.CreateElevator(startFloor);

        elevator.AddFloorRequest(requestedFloor);

        elevator.RequestedDirection.Should().Be(expected);
    }
}