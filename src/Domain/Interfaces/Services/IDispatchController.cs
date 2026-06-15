public interface IDispatchController
{
    public IElevator? FindBestElevator(List<IElevator> elevators, int currentFloor, ElevatorDirection requestedDirection);
}