using MediatR;

public class DisambarkPassengersCommandHandler : IRequestHandler<DisambarkPassengersCommand, Unit>
{
        private readonly IElevatorRepo _elevatorRepo;
    private readonly IFloorRepo _floorRepo;

    public DisambarkPassengersCommandHandler(IElevatorRepo elevatorRepo, IFloorRepo floorRepo)
    {
        _elevatorRepo = elevatorRepo;
        _floorRepo = floorRepo;
    }

    public Task<Unit> Handle(DisambarkPassengersCommand command, CancellationToken token)
    {
        

        return Unit.Task;
    }
}