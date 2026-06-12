using FluentAssertions;


public class BuildingTest
{
    private Building CreateBuilding() => new Building();
    private  PassengerElevatorFactory _elevatorFactory = new PassengerElevatorFactory();

    private List<IElevator> CreateElevators(int numberOfElevators = 2) => Enumerable.Range(0, numberOfElevators).Select(_ => _elevatorFactory.CreateElevator()).ToList();


    [Fact]
    public void Initialise_HasCorrectNumberOfFloors()
    {
        var building = CreateBuilding();

        building.Initialise(10, CreateElevators());

        building.Floors.Should().HaveCount(10);
    }

    [Fact]
    public void Initialise_RegisteredAllElevators()
    {
        var building = CreateBuilding();
        var elevators = CreateElevators(3);

        building.Initialise(5, elevators);

        building.Elevators.Should().HaveCount(3);
    }

    [Fact]
    public void Initialise_IsIitialiseTrue()
    {
        var building = CreateBuilding();

        building.Initialise(10, CreateElevators());

        building.IsInitialised.Should().BeTrue();
    }


    [Fact]
    public void Initialise_CannotInitilaiseZeroFloors()
    {
         var building = CreateBuilding();

       Action act = () => building.Initialise(0, CreateElevators());

       act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetFloor_ValidFloorReturned()
    {
        var building = CreateBuilding();

        building.Initialise(10, CreateElevators());

        var floor = building.GetFloor(7);

        floor.Should().NotBeNull();
    }

    [Fact]
    public void GetFloor_InvalidFloor_Exception()
    {
         var building = CreateBuilding();

        building.Initialise(10, CreateElevators());

        Action act = () =>  building.GetFloor(12);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }


    [Fact]
    public void GetFloor_BeforeInitialise_ThrowsInvalidOperationException()
    {
        var building = CreateBuilding();

        // building exists but Initialise() was never called
        Action act = () => building.GetFloor(1);

        act.Should().Throw<InvalidOperationException>().WithMessage("*initialised*");
    }

    [Fact]
    public void Initialise_CalledTwice_ReplacesFloorsAndElevators()
    {
        var building = CreateBuilding();
        building.Initialise( 5, CreateElevators(2));

        // reinitialise with different values
        building.Initialise( 10, CreateElevators(3));

        building.Floors.Should().HaveCount(10);
        building.Elevators.Should().HaveCount(3);
    }


}