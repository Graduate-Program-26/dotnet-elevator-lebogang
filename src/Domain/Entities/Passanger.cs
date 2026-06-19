public class Passenger
{
    public decimal WaitingTime {get; set;} = 0;
    public ElevatorDirection? RequestedDirection
    {
        get
        {
            return SourceFloor < DestinationFloor ? ElevatorDirection.Up : ElevatorDirection.Down;
        } set;
    }
    public PassengerStatus Status {get; set;} = PassengerStatus.Waiting;

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
        if(Status == PassengerStatus.Waiting)  throw new InvalidOperationException("Cannot disembark if waiting");
        Status = PassengerStatus.Arrived;
    }
}