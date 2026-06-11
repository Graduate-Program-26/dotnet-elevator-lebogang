using MediatR;

public class BoardPassengersCommandHandler : IRequestHandler<BoardPassengersCommand, Unit>
{
    
    private readonly IElevatorRepo _elevatorRepo;
    private readonly IFloorRepo _floorRepo;

    public BoardPassengersCommandHandler(IElevatorRepo elevatorRepo, IFloorRepo floorRepo)
    {
        _elevatorRepo = elevatorRepo;
        _floorRepo = floorRepo;
    }

    public Task<Unit> Handle(BoardPassengersCommand command, CancellationToken token)
    {
        

        return Unit.Task;
    }
}