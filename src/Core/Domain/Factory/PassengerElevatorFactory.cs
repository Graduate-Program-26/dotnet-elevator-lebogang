public class PassengerElevatorFactory : IElevatorFactory
{
    public override IElevator CreateElevator()
    {
        return  new PassengerElevator(10);
    }
}