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
        if(Status == PassengerStatus.InTransit)
       
        Status = PassengerStatus.InTransit;
    }

    public void Disembark()
    {
        Status = PassengerStatus.Arrived;
    }
}