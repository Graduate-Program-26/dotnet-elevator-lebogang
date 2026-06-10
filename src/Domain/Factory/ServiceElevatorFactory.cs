public class ServiceElevatorFactory : IElevatorFactory
{
    public override IElevator CreateElevator()
    {
        return new ServiceElevator(20);
    }
}