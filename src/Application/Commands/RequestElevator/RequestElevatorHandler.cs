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

        if(command.FloorNumber < 0) throw new InvalidElevatorRequestException();
        
        var elevator = _dispatchController.FindBestElevator(_elevatorRepo.GetElevators().ToList(), command.FloorNumber, command.Direction);
        
        if(elevator is null)
        {
            // retry and wait
            return Unit.Task;
        }
        
        elevator.AddFloorRequest(command.FloorNumber);
        return Unit.Task;
    }
}