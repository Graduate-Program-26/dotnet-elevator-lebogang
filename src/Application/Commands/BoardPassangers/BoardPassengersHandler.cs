using MediatR;

public class BoardPassengersCommandHandler : IRequestHandler<BoardPassengersCommand, Unit>
{
    

    public Task<Unit> Handle(BoardPassengersCommand command, CancellationToken token)
    {
        

        return Unit.Task;
    }
}