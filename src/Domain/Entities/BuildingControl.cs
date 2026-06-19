public class Building
{
    // needs to be injected as a singleton at DI

    private readonly List<Floor> _floors = new();
    public IReadOnlyList<Floor> Floors => _floors.AsReadOnly();
    private readonly List<IElevator> _elevators = new();
    public IReadOnlyList<IElevator> Elevators => _elevators.AsReadOnly();

    public bool IsInitialised {get; private set;} = false;

  
    public Building() {}


    public void Initialise(int totalFloors, IEnumerable<IElevator> elevators)
    {
        if(totalFloors <= 1)
        {
            throw new ArgumentOutOfRangeException("Cannot initialise floor with floors less than or equal rto zero");
        }
        
        _floors.Clear();

        for (int i = 0; i < totalFloors; i++)
        {
            _floors.Add(new Floor(i));
        }

        _elevators.Clear();
        _elevators.AddRange(elevators);

        IsInitialised = true;
    }


    public Floor GetFloor(int floorNumber)
    {
        if(!IsInitialised)     throw new InvalidOperationException("Building has not been initialised.");
        

        if(floorNumber > Floors.Count()) throw new ArgumentOutOfRangeException("Floor number is not valid on this building");


        var floor = _floors.FirstOrDefault(floor => floor.FloorNumber == floorNumber) ?? throw new ArgumentOutOfRangeException(nameof(floorNumber));
        return floor;
    }

}