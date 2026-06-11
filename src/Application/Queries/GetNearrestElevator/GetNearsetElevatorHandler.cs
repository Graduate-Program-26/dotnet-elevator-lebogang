using MediatR;
public class GetNearestElevatorQueryHandler : IRequestHandler<GetNearestElevatorQuery, IElevator>
{
    
    public Task<IElevator> Handle(GetNearestElevatorQuery query, CancellationToken token)
    {
        

        // inject fetching logic

    }
}