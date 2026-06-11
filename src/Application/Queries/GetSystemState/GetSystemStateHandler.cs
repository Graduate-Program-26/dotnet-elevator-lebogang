
using MediatR;

public class GetSystemStateQueryHandler : IRequestHandler<GetSystemStateQuery, SystemState>
{
    
    private readonly IElevatorRepo _elevatorRepo;
    private readonly IFloorRepo _floorRepo;

    public GetSystemStateQueryHandler(IElevatorRepo elevatorRepo, IFloorRepo floorRepo)
    {
        _elevatorRepo = elevatorRepo;
        _floorRepo = floorRepo;
    }

    
    public Task<SystemState> Handle(GetSystemStateQuery query, CancellationToken token)
    {
        
    }
}