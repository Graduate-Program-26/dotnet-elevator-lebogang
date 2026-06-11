
public class GlassElevator : ElevatorBase
{

    public override int MaxCapacity {get; }

    public GlassElevator(int maxCapacity)
    {  
        MaxCapacity = maxCapacity;
    }


}