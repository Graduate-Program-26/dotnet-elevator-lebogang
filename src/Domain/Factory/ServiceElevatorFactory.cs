public class ServiceElevatorFactory : IElevatorFactory
{
        private readonly int _MAX_CAPCITY = 20;

    public override IElevator CreateElevator(int startingFloor = 0)
    {
        return new ServiceElevator(_MAX_CAPCITY, startingFloor);
    }
}