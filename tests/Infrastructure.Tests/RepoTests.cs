using FluentAssertions;
using Moq;


public class RepoTests
{
    private static PassengerElevatorFactory _passengerElevatorFactory = new PassengerElevatorFactory();


    private static (Building building,IElevatorRepo elevatorRepo,IFloorRepo floorRepo) BuildSeededRepos(int floorCount = 4, int elevatorCount = 3)
    {
        var building = new Building();
        var elevators = Enumerable.Range(0, elevatorCount).Select(_ => _passengerElevatorFactory.CreateElevator()).ToList();

        building.Initialise(floorCount, elevators);

        IElevatorRepo elevatorRepo = new InMemoryElevatorRepo(building.Elevators.ToList());
        IFloorRepo floorRepo = new InMemoryFloorRepo(building.Floors.ToList());

        return (building, elevatorRepo, floorRepo);
    }

    [Fact]
    public void ElevatorRepo_AfterSeed_ContainsAllBuildingElevators()
    {
        Building building = new Building();
        var elevators = new List<IElevator> { _passengerElevatorFactory.CreateElevator(), _passengerElevatorFactory.CreateElevator(), _passengerElevatorFactory.CreateElevator() };

        var floorCount = 4;
        building.Initialise(floorCount, elevators);
        IElevatorRepo inMemoryElevatorRepo = new InMemoryElevatorRepo(building.Elevators.ToList());
        IFloorRepo inMemoryFloorRepo = new InMemoryFloorRepo(building.Floors.ToList());


        inMemoryElevatorRepo.GetElevators().Count.Should().Be(3);

        inMemoryFloorRepo.GetFloors().Count.Should().Be(floorCount);
    }

    [Fact]
    public void FloorRepo_GetFloorWIthInvalidNumber_ThrowException()
    {
        var (_, _, floorRepo) = BuildSeededRepos(floorCount: 4);
    
        Action act = () => floorRepo.GetFloor(100);

        act.Should().Throw<InvalidFloorNumberExecption>();
    }

    [Fact]
    public void ElevatorRepo_GetElevatorById_ReturnsCorrectElevator()
    {
        var (building, elevatorRepo, _) = BuildSeededRepos();
        var expected = building.Elevators.First();

        var result = elevatorRepo.GetElevator(expected.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(expected.Id);
    }

    // these tests proves the singleton shared-reference guarantee
    [Fact]
    public void ElevatorRepo_WhenElevatorMutated_RepoReflectsChange()
    {
        var (building, elevatorRepo, _) = BuildSeededRepos();
        var elevator = building.Elevators.First();

        // simulate what MoveElevatorCommandHandler does
        elevator.AddFloorRequest(5);
        elevator.MoveOneFloor();

        var retrieved = elevatorRepo.GetElevator(elevator.Id);
        retrieved!.CurrentFloor.Should().Be(1); 
    }

    [Fact]
    public void FloorRepo_WhenPassengerAdded_RepoReflectsChange()
    {
        // same shared-reference guarantee for floors
        var (building, _, floorRepo) = BuildSeededRepos(floorCount: 4);
        var floor = building.Floors.First(f => f.FloorNumber == 2);

        floor.AddWaitingPassenger(new Passenger(sourceFloor: 2, destinationFloor: 7));

        var retrieved = floorRepo.GetFloor(2);
        retrieved.WaitingPassengers.Should().HaveCount(1);
    }
}

