public class HighSpeedElavator : ElevatorBase
{

    public override int MaxCapacity { get; }
    public HighSpeedElavator(int maxCapacity)
    {

      
        MaxCapacity = maxCapacity;
    }
}