public class Passenger
{
    public decimal WaitingTime {get; set;} = 0;
    public string? RequestedDirection {get; set;}
    public PassengerStatus Status {get; set;} = PassengerStatus.Waitng;

    public Passenger(int sourceFloor, int destinationFloor)
    {
        
    }
}