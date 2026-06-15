using MediatR;

public class RequestElevatorCommandHandler : IRequestHandler<RequestElevatorCommand, Unit>
{
    
    private readonly IElevatorRepo _elevatorRepo;
    private readonly IFloorRepo _floorRepo;

    private readonly IDispatchController _dispatchController;

    public RequestElevatorCommandHandler(IElevatorRepo elevatorRepo, IFloorRepo floorRepo, IDispatchController dispatchController)
    {
        _elevatorRepo = elevatorRepo;
        _floorRepo = floorRepo;
        _dispatchController = dispatchController;
    }
    public Task<Unit> Handle(RequestElevatorCommand command, CancellationToken token)
    {
        

        return Unit.Task;
    }
}