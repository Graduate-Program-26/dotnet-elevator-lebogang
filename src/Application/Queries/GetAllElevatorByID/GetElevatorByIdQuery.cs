using MediatR;

public record GetElevatorStateQuery(Guid Id) : IRequest<ElevatorStateDto>;