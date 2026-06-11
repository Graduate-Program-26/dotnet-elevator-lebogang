

using MediatR;

public class AssignElevatorCommandHandler : IRequestHandler<AssignElevatorCommand, Unit>
{
    

    public Task<Unit> Handle(AssignElevatorCommand command, CancellationToken token )
    {
        

        return Unit.Task;
    }
}