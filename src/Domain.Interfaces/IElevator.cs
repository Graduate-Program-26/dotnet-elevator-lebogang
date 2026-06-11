

public interface IElevator
{
   public Guid Id {get;}
    public int MaxCapacity {get; }
    public int CurrentCapacity {get; }

    public int CurrentFloor {get;}

    /// <summary>
    ///  by default there is no requested direction
    /// </summary>
    public ElevatorDirection? RequestedDirection {get; set;}

    public Queue<int> FloorRequests {get; set;}


}