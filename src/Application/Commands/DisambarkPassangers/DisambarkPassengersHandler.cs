using MediatR;

public class DisambarkPassengersCommandHandler : IRequestHandler<DisambarkPassengersCommand, Unit>
{
    

    public Task<Unit> Handle(DisambarkPassengersCommand command, CancellationToken token)
    {
        

        return Unit.Task;
    }
}