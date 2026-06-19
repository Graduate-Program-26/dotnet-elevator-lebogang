using FluentAssertions; 
using Xunit;

public class PassengerTest
{
    private Passenger CreatePassenger(int source = 0, int destination = 5) => new Passenger(source, destination);

    [Fact]
    public void Passenger_InitialStatus_IsWaiting()
    {
        var passenger = CreatePassenger();

        passenger.Status.Should().Be(PassengerStatus.Waiting);
    }


    [Fact]
    public void Board_ChangesStatus_InTransit()
    {
        var passenger = CreatePassenger();

        passenger.Board();

        passenger.Status.Should().Be(PassengerStatus.InTransit);
    }


    [Fact]
    public void Arrive_ChangesStatus_ToArrived()
    {
        var passenger = CreatePassenger();
        passenger.Board();

        passenger.Disembark();

        passenger.Status.Should().Be(PassengerStatus.Arrived);
    }

    [Fact]
    public void Board_ChangesStatus_ExceptionWhen_BoardingInTransit()
    {
        var passenger = CreatePassenger();

        passenger.Board(); // In transit here
        Action act = () =>  passenger.Board();

        act.Should().Throw<InvalidOperationException>().WithMessage("*No boarding as passanger already in transit*");

    }

    [Fact]
    public void Board_ChangesStatus_ExceptionWhen_DisembarkingWHileWaiting()
    {
        var passenger = CreatePassenger();

        Action act = () =>  passenger.Disembark();

        act.Should().Throw<InvalidOperationException>().WithMessage("*Cannot disembark if waiting*");

    }

}