public class PassengerElevator : ElevatorBase
{

    public override int MaxCapacity {get;}
    public PassengerElevator(int maxCapacity)
    {
        MaxCapacity = maxCapacity; 
    }
}