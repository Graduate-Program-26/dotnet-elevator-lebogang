public class GlassElevatorFactory : IElevatorFactory
{
    public override IElevator CreateElevator()
    {
        return new GlassElevator(5);
    }
}