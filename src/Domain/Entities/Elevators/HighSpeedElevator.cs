public class HighSpeedElavator : ElevatorBase
{

    public HighSpeedElavator(int maxCapacity, int startingFloor): base(maxCapacity, startingFloor)
    {

    }

    /// <summary>
    /// moves t floors per simulation tikc
    /// </summary>
    public override void MoveOneFloor()
    {
        var floorsBefore = FloorRequests.Count;

        base.MoveOneFloor(); // first floor

        // only move the second floor if we did not just arrive somewhere
        var arrivedOnFirstMove = FloorRequests.Count < floorsBefore;

        if (!arrivedOnFirstMove) base.MoveOneFloor(); 
    }
}