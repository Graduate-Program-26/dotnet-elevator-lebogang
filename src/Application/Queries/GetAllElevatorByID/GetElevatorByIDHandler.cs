using MediatR;

public class GetElevatorStateQueryHandler : IRequestHandler<GetElevatorStateQuery, ElevatorStateDto>
{
    
    private readonly IElevatorRepo _elevatorRepo;
    private readonly IFloorRepo _floorRepo;

    public GetElevatorStateQueryHandler(IElevatorRepo elevatorRepo, IFloorRepo floorRepo)
    {
        _elevatorRepo = elevatorRepo;
        _floorRepo = floorRepo;
    }


    public Task<ElevatorStateDto> Handle(GetElevatorStateQuery query, CancellationToken token)
    {
        
    }
}