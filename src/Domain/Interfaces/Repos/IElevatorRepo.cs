
/// <summary>
/// Defines the data access contract for elevator state.
/// Implementations may be in-memory, persistent, or distributed.
/// The Application layer depends only on this contract, never on
/// a concrete implementation.
/// </summary>
public interface IElevatorRepo
{
    IReadOnlyList<IElevator> GetElevators();

    IElevator? GetElevator(Guid Id);

    void Update(ElevatorBase elevator);
}