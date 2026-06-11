
using MediatR;

public class GetSystemStateQueryHandler : IRequestHandler<GetSystemStateQuery, SystemState>
{
    

    public Task<SystemState> Handle(GetSystemStateQuery query, CancellationToken token)
    {
        
    }
}