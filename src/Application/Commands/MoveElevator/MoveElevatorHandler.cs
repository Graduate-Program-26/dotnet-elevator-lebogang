

using MediatR;

public class MoveElevatorCommandHandler : IRequestHandler<MoveElevatorCommand, Unit>
{
    
     private readonly IElevatorRepo _elevatorRepo;
    private readonly IFloorRepo _floorRepo;
    private readonly IDispatchController _dispatchController;
    public MoveElevatorCommandHandler(IElevatorRepo elevatorRepo, IFloorRepo floorRepo, IDispatchController dispatchController)
    {
        _elevatorRepo = elevatorRepo;
        _floorRepo = floorRepo;
        _dispatchController = dispatchController;
    }



    public Task<Unit> Handle(MoveElevatorCommand command, CancellationToken token )
    {
        

        return Unit.Task;
    }
}