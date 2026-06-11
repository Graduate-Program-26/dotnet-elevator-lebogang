using MediatR;

public class RequestElevatorCommandHandler : IRequestHandler<RequestElevatorCommand, Unit>
{
    

    public Task<Unit> Handle(RequestElevatorCommand command, CancellationToken token)
    {
        

        return Unit.Task;
    }
}