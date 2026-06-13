public class Passenger
{
    public decimal WaitingTime {get; set;} = 0;
    public string? RequestedDirection {get; set;}
    public PassengerStatus Status {get; set;} = PassengerStatus.Waitng;

    public int SourceFloor {get;}
    public int DestinationFloor {get;}
    public Passenger(int sourceFloor, int destinationFloor)
    {
        SourceFloor = sourceFloor;
        DestinationFloor = destinationFloor;
    }


    public void Board()
    {
        if(Status == PassengerStatus.InTransit) throw new InvalidOperationException("No boarding as passanger already in transit");
       
        Status = PassengerStatus.InTransit;
    }

    public void Disembark()
    {
        if(Status == PassengerStatus.Waitng)  throw new InvalidOperationException("Cannot disembark if waiting");
        Status = PassengerStatus.Arrived;
    }
}