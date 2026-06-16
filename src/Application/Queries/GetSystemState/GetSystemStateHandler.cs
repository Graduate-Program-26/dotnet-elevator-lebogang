
using MediatR;

public class GetSystemStateQueryHandler : IRequestHandler<GetSystemStateQuery, SystemStatusDto>
{
    
    private readonly IElevatorRepo _elevatorRepo;
    private readonly IFloorRepo _floorRepo;

    public GetSystemStateQueryHandler(IElevatorRepo elevatorRepo, IFloorRepo floorRepo)
    {
        _elevatorRepo = elevatorRepo;
        _floorRepo = floorRepo;
    }

    
    public Task<SystemStatusDto> Handle(GetSystemStateQuery query, CancellationToken token)
    {
        var elevators = _elevatorRepo.GetElevators();
        var floors = _floorRepo.GetFloors();

        // Object to DTO projetcion mapping, thread safe and defends agains memeory mutaion as this is being sent out to the public system
        var systemStatus = new SystemStatusDto(
            Elevators : elevators.Select(MapElavators).ToList(),
            Floors: floors.Select(MapFloors).ToList()
        );


        return Task.FromResult(systemStatus);
    }

    private static ElevatorStatusDto MapElavators(IElevator elevator)
    {
        return new(
            Id:               elevator.Id,
            CurrentFloor:     elevator.CurrentFloor,
            Direction:        elevator.RequestedDirection ?? ElevatorDirection.Up,
            State:            elevator.State,
            CurrentCapacity:  elevator.CurrentCapacity,
            MaxCapacity:      elevator.MaxCapacity,
            FloorRequests:    elevator.FloorRequests.ToList()
        );
    }

    private static FloorStatusDto MapFloors(Floor floor)
    {
        return new(
            FloorNumber: floor.FloorNumber,
            WaitingPassengerCount: floor.WaitingPassengers.Count
        );
    }
}