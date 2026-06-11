public class GlassElevatorFactory : IElevatorFactory
{
    private readonly int _MAX_CAPCITY = 5;

    public override IElevator CreateElevator(int startingFloor = 0)
    {
        return new GlassElevator(_MAX_CAPCITY, startingFloor);
    }
}