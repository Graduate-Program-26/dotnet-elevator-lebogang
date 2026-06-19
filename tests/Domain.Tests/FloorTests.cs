using FluentAssertions; 
using Xunit;

public class FloorTest
{
    private Floor CreateFloor(int floorNumber = 1)  => new Floor(floorNumber);
    private Passenger CreatePassenger(int source = 0, int destination = 5) => new Passenger(source, destination);

    [Fact]
    public void Floor_InitialisesWithCorrectFloorNumber()
    {
        var floor = CreateFloor(3);

        floor.FloorNumber.Should().Be(3);
    }

    [Fact]
    public void Floor_InitialisesWithNoWaitingPassengers()
    {
        var floor = CreateFloor();

        floor.WaitingPassengers.Should().BeEmpty();
    }


    [Fact]
    public void AddWaitingPassenger_ValidPassenger_PassengerAppearsInList()
    {
        var floor = CreateFloor(1);

        var passenger = CreatePassenger(1);

        floor.AddWaitingPassenger(passenger);


        floor.WaitingPassengers.Should().Contain(passenger);
    }

    [Fact]
    public void RemoveWaitingPassenger_ExistingPassenger_PassengerNoLongerInList()
    {
        var floor = CreateFloor(1);

        var passenger = CreatePassenger(1);

        floor.AddWaitingPassenger(passenger);


        floor.RemoveWaitingPassenger(passenger);

        floor.WaitingPassengers.Should().NotContain(passenger);
    }

    [Fact]
    public void AddWaitingPassenger_WrongSourceFloor_ThrowsInvalidPassengerException()
    {
        var floor = CreateFloor(1);

        var passenger = CreatePassenger(2); 

       Action act = () => floor.AddWaitingPassenger(passenger);


        act.Should().Throw<InvalidPassengerAddedToFloor>().WithMessage("*added*");

    }

    [Fact]
    public void AddWaitingPassenger_NullPassenger_ThrowsArgumentNullException()
    {
        var floor = CreateFloor(1);

       Action act = () => floor.AddWaitingPassenger(null);


        act.Should().Throw<ArgumentNullException>();


    }


    [Fact]
    public void HasWaitingPassengers_WhenPassengerAdded_ReturnsTrue()
    {
        var floor = CreateFloor(1);
        floor.AddWaitingPassenger(CreatePassenger(source: 1));

        floor.HasWaitingPassengers.Should().BeTrue();
    }

    [Fact]
    public void HasWaitingPassengers_AfterAllRemoved_ReturnsFalse()
    {
        var floor = CreateFloor(1);
        var passenger = CreatePassenger(source: 1);
        floor.AddWaitingPassenger(passenger);
        floor.RemoveWaitingPassenger(passenger);

        floor.HasWaitingPassengers.Should().BeFalse();
    }
}