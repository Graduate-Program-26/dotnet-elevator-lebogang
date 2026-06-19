public class ServiceElevator : ElevatorBase
{
    private bool _skipThisTick = false;
    
    // more capacity but slower
    public ServiceElevator(int maxCapacity,int startingFloor): base(maxCapacity, startingFloor)
    {
     
    }

    public override void MoveOneFloor()
    {
        if (_skipThisTick)
        {
            _skipThisTick = false; // next tick will move
            return;
        }

        _skipThisTick = true; // next tick will skip
        base.MoveOneFloor();
    }
}