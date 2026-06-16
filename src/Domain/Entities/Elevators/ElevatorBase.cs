


public abstract class ElevatorBase : IElevator
{
    public Guid Id { get; } = Guid.NewGuid();
    public int MaxCapacity { get; }
    public int CurrentCapacity { 
        get
        {
          return   _onboardPassengers.Count;
        } set;
    } 

    public int CurrentFloor { get; protected set; }


    private readonly List<Passenger> _onboardPassengers = new();
    public IReadOnlyList<Passenger> OnboardPassengers => _onboardPassengers.AsReadOnly();
    public ElevatorDirection RequestedDirection
    {
        get
        {
            if (_floorRequests.Count == 0) return ElevatorDirection.Stationary;

            var nextRequest = _floorRequests.Min;

            if (nextRequest > CurrentFloor) return ElevatorDirection.Up;
            if (nextRequest < CurrentFloor) return ElevatorDirection.Down;

            return ElevatorDirection.Stationary;
        }
    }


    // SortedSet prevents duplicates and keeps floors ordered automatically
    private readonly SortedSet<int> _floorRequests = new();
    public IReadOnlyCollection<int> FloorRequests => _floorRequests;
    public ElevatorState State { get; set; } = ElevatorState.Idle;
    protected ElevatorBase(int maxCapacity, int startingFloor)
    {
        MaxCapacity = maxCapacity;
        CurrentFloor = startingFloor;
    }

    public void AddFloorRequest(int floorNumber)
    {
        _floorRequests.Add(floorNumber);

        if (State == ElevatorState.Idle) State = ElevatorState.Traveling;
    }

    public virtual void MoveOneFloor()
    {
        if (_floorRequests.Count == 0) return; /// there is no jobs, do nothing

        if (RequestedDirection == ElevatorDirection.Up) CurrentFloor++;
        else if (RequestedDirection == ElevatorDirection.Down) CurrentFloor--;


        if (_floorRequests.Contains(CurrentFloor))
        {
            _floorRequests.Remove(CurrentFloor);
            // stop on this floor which passengers would be boarded onto 

            // if its the last serviced request, idle on that floor
            if (_floorRequests.Count == 0)
            {
                State = ElevatorState.Idle;
               
            }
        }
    }


    public void BoardPassengers(int passengerCount)
    {
        if (CurrentCapacity + passengerCount > MaxCapacity)
        {
            throw new ElevatorCapacityException(MaxCapacity);
        }

        CurrentCapacity += passengerCount;
    }

    public void DisembarkPassengers(int passengerCount)
    {
        if (CurrentCapacity - passengerCount < 0)
        {
            // disembark all, not negavtive capacity
            CurrentCapacity = 0;
            return;
        }

        CurrentCapacity -= passengerCount;
    }

    public void BoardPassengers(IEnumerable<Passenger> passengers)
    {
        var list = passengers.ToList();

        if(list.Count == 0) return;

        if (CurrentCapacity + list.Count > MaxCapacity) throw new ElevatorCapacityException(MaxCapacity);

        foreach (var passenger in list)
        {
            _onboardPassengers.Add(passenger);
            passenger.Board();
            AddFloorRequest(passenger.DestinationFloor);

        }
    }

    public List<Passenger> DisembarkAtCurrentFloor()
    {
        var arriving = _onboardPassengers.Where(passenger => passenger.DestinationFloor == CurrentFloor).ToList();

        _onboardPassengers.RemoveAll(arriving.Contains);
        return arriving;
    }

    public bool HasCapacity()
    {
        return CurrentCapacity < MaxCapacity;
    }
}