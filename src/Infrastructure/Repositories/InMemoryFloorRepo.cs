public class InMemoryFloorRepo : IFloorRepo
{
    private List<Floor> _floors  = new ();
    
    public InMemoryFloorRepo(List<Floor> floors)
    {
        _floors = floors;
    }

    public Floor GetFloor(int floorNumber)
    {
       if(floorNumber < 0 || floorNumber > _floors.Count) throw new InvalidFloorNumberExecption(floorNumber);
       
       return _floors.FirstOrDefault(floor => floor.FloorNumber == floorNumber);
    }

    public IReadOnlyList<Floor> GetFloors()
    {
       return _floors;
    }

    public void Update(Floor floor)
    {
        
    }
}