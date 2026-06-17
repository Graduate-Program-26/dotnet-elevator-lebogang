public class InvalidElevatorIdException : Exception
{
    public InvalidElevatorIdException(Guid id) : base($"Elevator with Id {id} does not exist")
    {
        
    }
}