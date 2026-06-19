public class PassengerElevatorFactory : IElevatorFactory
{
        private readonly int _MAX_CAPCITY = 10;

    public override IElevator CreateElevator(int startingFloor = 0)
    {
        return  new PassengerElevator(_MAX_CAPCITY, startingFloor);
    }
}