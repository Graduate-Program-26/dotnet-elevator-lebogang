using MediatR;
public class GetNearestElevatorQueryHandler : IRequestHandler<GetNearestElevatorQuery, IElevator>
{
     private readonly IElevatorRepo _elevatorRepo;
    private readonly IFloorRepo _floorRepo;

    public GetNearestElevatorQueryHandler(IElevatorRepo elevatorRepo, IFloorRepo floorRepo)
    {
        _elevatorRepo = elevatorRepo;
        _floorRepo = floorRepo;
    }



    public Task<IElevator> Handle(GetNearestElevatorQuery query, CancellationToken token)
    {
        

        // inject fetching logic

    }
}