public class InMemoryElevatorRepo : IElevatorRepo
{
    private List<IElevator> _elevators = new();
    
    public InMemoryElevatorRepo(List<IElevator> elevators)
    {
        _elevators = elevators;
    }


    public IElevator? GetElevator(Guid Id)
    {
       return _elevators.FirstOrDefault(elevator => elevator.Id == Id);
    }

    public IReadOnlyList<IElevator> GetElevators()
    {
        return _elevators;
    }

    public void Update(ElevatorBase elevator)
    {
        
    }
}