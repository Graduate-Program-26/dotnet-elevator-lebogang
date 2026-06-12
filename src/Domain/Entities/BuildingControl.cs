public class Building
{

    private static Building _buildingInstance ; /// Singleton
    private readonly List<Floor> _floors ;
    public IReadOnlyList<Floor> Floors => _floors.AsReadOnly();
    private readonly List<IElevator> _elevators;
    public IReadOnlyList<IElevator> Elevators => _elevators.AsReadOnly();



    public static Building GetBuildingInstance()
    {
        if(_buildingInstance == null)
        {
            _buildingInstance = new Building();

        }

        return _buildingInstance;
    }

    public Building() {}


    public void Initialise(int totalFloors, IEnumerable<IElevator> elevators)
    {
        _floors.Clear();

        for (int i = 0; i < totalFloors; i++)
        {
            _floors.Add(new Floor(i));
        }

        _elevators.Clear();
        _elevators.AddRange(elevators);
    }


    public Floor GetFloor(int floorNumber)
    {
        var floor = _floors.FirstOrDefault(floor => floor.FloorNumber == floorNumber) ?? throw new ArgumentOutOfRangeException(nameof(floorNumber));
        return floor;
    }


    public void StartEngine()
    {
        
    }
}