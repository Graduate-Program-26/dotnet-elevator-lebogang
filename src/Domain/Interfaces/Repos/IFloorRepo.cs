/// <summary>
/// Defines the data access contract for floot state.
/// Implementations may be in-memory, persistent, or distributed.
/// The Application layer depends only on this contract, never on
/// a concrete implementation.
/// </summary>
public interface IFloorRepo
{
    IReadOnlyList<Floor> GetFloors();

    Floor GetFloor(int floorNumber);


    void Update(Floor floor);
}