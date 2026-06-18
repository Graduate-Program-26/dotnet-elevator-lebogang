using MediatR;

public class RegisterPassengerCommandHandler: IRequestHandler<RegisterPassengerCommand, Unit>
{
    private readonly IFloorRepo _floorRepo;

    public RegisterPassengerCommandHandler(IFloorRepo floorRepo)
    {
        _floorRepo = floorRepo;
    }
   

    public Task<Unit> Handle(RegisterPassengerCommand command, CancellationToken token)
    {
        var floor = _floorRepo.GetFloor(command.SourceFloor);
        var passenger = new Passenger(command.SourceFloor, command.DestinationFloor);

        floor.AddWaitingPassenger(passenger);
        return Unit.Task;
    }
}