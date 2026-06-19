public class HighSpeedElavatorFactory : IElevatorFactory
{
    private readonly int _MAX_CAPCITY = 8;
    public override IElevator CreateElevator(int startingFloor = 0)
    {
        return new HighSpeedElavator(_MAX_CAPCITY, startingFloor);
    }
}