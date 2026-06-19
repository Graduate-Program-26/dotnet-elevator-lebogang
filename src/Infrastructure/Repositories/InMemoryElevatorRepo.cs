using System.Collections.Concurrent;

public class InMemoryElevatorRepo : IElevatorRepo
{
     private readonly ConcurrentDictionary<Guid, IElevator> _elevators = new();

    
    public InMemoryElevatorRepo()
    {
        
    }

  public InMemoryElevatorRepo(IEnumerable<IElevator> elevators)
    {
        Seed(elevators.ToList());
    }
    public IElevator? GetElevator(Guid Id)
    {
       return _elevators.TryGetValue(Id, out var elevator) ? elevator : null;
    }

    public IReadOnlyList<IElevator> GetElevators()
    {
        return _elevators.Values.ToList().AsReadOnly();
    }

    public void Update(ElevatorBase elevator)
    {
        
    }

    public void Seed(IReadOnlyList<IElevator> elevators)
    {
        _elevators.Clear();

        foreach (var elevator in elevators)
            _elevators.TryAdd(elevator.Id, elevator);
    }

}