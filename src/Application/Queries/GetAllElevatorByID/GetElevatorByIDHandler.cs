using MediatR;

public class GetElevatorStateQueryHandler : IRequestHandler<GetElevatorStateQuery, ElevatorStateDto>
{
    
    private readonly IElevatorRepo _elevatorRepo;


    public GetElevatorStateQueryHandler(IElevatorRepo elevatorRepo)
    {
        _elevatorRepo = elevatorRepo;
    }


    public Task<ElevatorStateDto> Handle(GetElevatorStateQuery query, CancellationToken token)
    {
        var elevator = _elevatorRepo.GetElevator(query.Id);

        if(elevator is null) throw new InvalidElevatorIdException(query.Id);

        return Task.FromResult(MapElevators(elevator));
    }


    private static ElevatorStateDto MapElevators(IElevator elevator)
    {
        return new(
            Id: elevator.Id,
            CurrentFloor: elevator.CurrentFloor,
            Direction: elevator.RequestedDirection ?? ElevatorDirection.Up,
            State: elevator.State,
            CurrentCapacity: elevator.CurrentCapacity,
            MaxCapacity: elevator.MaxCapacity,
            FloorRequests: elevator.FloorRequests.ToList(),
            OnboardPassengerDestinations: elevator.OnboardPassengers.Select(passeneger => passeneger.DestinationFloor).ToList();
        );
    }
}