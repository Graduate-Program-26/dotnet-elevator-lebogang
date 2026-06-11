

using MediatR;

public class AssignElevatorCommandHandler : IRequestHandler<AssignElevatorCommand, Unit>
{
    
     private readonly IElevatorRepo _elevatorRepo;
    private readonly IFloorRepo _floorRepo;

    public AssignElevatorCommandHandler(IElevatorRepo elevatorRepo, IFloorRepo floorRepo)
    {
        _elevatorRepo = elevatorRepo;
        _floorRepo = floorRepo;
    }



    public Task<Unit> Handle(AssignElevatorCommand command, CancellationToken token )
    {
        

        return Unit.Task;
    }
}