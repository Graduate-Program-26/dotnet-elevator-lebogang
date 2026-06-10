public class ServiceElevator : ElevatorBase
{
    public override int MaxCapacity {get; }

    // more capacity but slower
    public ServiceElevator(int maxCapacity)
    {
        MaxCapacity = maxCapacity;
    }
}