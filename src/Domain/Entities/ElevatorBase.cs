

using System.Linq.Expressions;

public abstract class ElevatorBase : IElevator
{
    public  Guid Id { get; } = Guid.NewGuid();
    public   int MaxCapacity { get; }
    public  int CurrentCapacity { get; set; } = 0;
    public  int CurrentFloor { get; protected set; }
    public  ElevatorDirection? RequestedDirection { get; set; } = ElevatorDirection.Sationary;
    public  Queue<int> FloorRequests { get; set; } = new();
    public ElevatorState State {get; set;} = ElevatorState.Idle; 
    protected ElevatorBase(int maxCapacity, int startingFloor)
    {
        MaxCapacity = maxCapacity;
        CurrentFloor = startingFloor;
    }

    public void AddFloorRequest(int floorNumber)
    {
        
    }

    public void MoveOneFloor()
    {
        
    }


    public void BoardPassengers(int passengerCount)
    {
        
    }

    public bool HasCapacity()
    {
        
        return true;
    }
}