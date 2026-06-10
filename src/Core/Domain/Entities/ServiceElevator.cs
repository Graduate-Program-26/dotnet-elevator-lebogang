public class ServiceElevator : ElevatorBase
{
        public override int MaxCapacity {get; }

    public ServiceElevator(int maxCapacity)
    {
        MaxCapacity = maxCapacity;
    }
}