public abstract class ElevatorBase : IElevator
{
    public  Guid Id { get; } = Guid.NewGuid();
    public abstract  int MaxCapacity { get; }
    public  int CurrentCapacity { get; set; } = 0;
    public  int CurrentFloor { get; protected set; } = 0;
    public  ElevatorDirection? RequestedDirection { get; set; } = ElevatorDirection.Sationary;
    public  Queue<int> FloorRequests { get; set; } = new();
    public ElevatorState State {get; set;} = ElevatorState.Idle; 

}