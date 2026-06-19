using System.Collections.Concurrent;

public class InMemoryFloorRepo : IFloorRepo
{
     private readonly ConcurrentDictionary<int, Floor> _floors = new();
    
    public InMemoryFloorRepo()
    {
       
    }

    public InMemoryFloorRepo(IEnumerable<Floor> floors)
    {
        Seed(floors.ToList());
    }

    public Floor GetFloor(int floorNumber)
    {
       if(floorNumber < 0 || floorNumber > _floors.Count) throw new InvalidFloorNumberExecption(floorNumber);
       
       return _floors.TryGetValue(floorNumber, out var floor) ? floor: throw new ArgumentOutOfRangeException( nameof(floorNumber),$"Floor {floorNumber} does not exist");
    }

    public IReadOnlyList<Floor> GetFloors()
    {
       return _floors.Values.OrderBy(f => f.FloorNumber).ToList().AsReadOnly();
    }

    public void Update(Floor floor)
    {
        
    }

    public void Seed(IReadOnlyList<Floor> floors)
    {
        _floors.Clear();

        foreach (var floor in floors)
            _floors.TryAdd(floor.FloorNumber, floor);
    }

}