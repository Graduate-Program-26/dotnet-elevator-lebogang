using MediatR;

public class RequestElevatorCommandHandler : IRequestHandler<RequestElevatorCommand, Unit>
{
    
        private readonly IElevatorRepo _elevatorRepo;
    private readonly IFloorRepo _floorRepo;

    public RequestElevatorCommandHandler(IElevatorRepo elevatorRepo, IFloorRepo floorRepo)
    {
        _elevatorRepo = elevatorRepo;
        _floorRepo = floorRepo;
    }
    public Task<Unit> Handle(RequestElevatorCommand command, CancellationToken token)
    {
        

        return Unit.Task;
    }
}