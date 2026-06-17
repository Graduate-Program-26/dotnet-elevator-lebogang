

public interface IElevator
{
   public Guid Id {get;}
    public int MaxCapacity {get; }
    public int CurrentCapacity {get; }

    public int CurrentFloor {get;}

      public IReadOnlyList<Passenger> OnboardPassengers {get;}
    /// <summary>
    ///  by default there is no requested direction
    /// </summary>
    public ElevatorDirection RequestedDirection {get;  }

     public IReadOnlyCollection<int> FloorRequests {get; }
    public void AddFloorRequest(int floorNumber);

    public void MoveOneFloor();

    public void BoardPassengers(int passengerCount);
     public void BoardPassengers(IEnumerable<Passenger> passengers);
      public List<Passenger> DisembarkAtCurrentFloor();
    public void DisembarkPassengers(int passengerCount);
    
    public ElevatorState State {get; set;}

    public bool HasCapacity();
}