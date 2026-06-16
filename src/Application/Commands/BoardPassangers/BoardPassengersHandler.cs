using MediatR;

public class BoardPassengersCommandHandler : IRequestHandler<BoardPassengersCommand, Unit>
{

    private readonly IFloorRepo _floorRepo;
    public BoardPassengersCommandHandler(IFloorRepo floorRepo)
    {
        _floorRepo = floorRepo;
    }

    public Task<Unit> Handle(BoardPassengersCommand command, CancellationToken token)
    {
        List<Passenger> boardingPassengers = new();
        int spaceAvailabie = command.Elevator.MaxCapacity - command.Elevator.CurrentCapacity;

        var floor = _floorRepo.GetFloor(command.Elevator.CurrentFloor);

        foreach (var pasenger in command.WaitingPassangers.ToList())
        {
            if (spaceAvailabie == 0) break;

            boardingPassengers.Add(pasenger);
            command.WaitingPassangers.Remove(pasenger);
            floor.RemoveWaitingPassenger(pasenger);
            spaceAvailabie--;
        }

        command.Elevator.BoardPassengers(boardingPassengers);

        return Unit.Task;
    }
}