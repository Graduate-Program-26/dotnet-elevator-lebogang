public class HighSpeedElavatorFactory : IElevatorFactory
{
    public override IElevator CreateElevator()
    {
        return new HighSpeedElavator(8);
    }
}