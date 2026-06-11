using MediatR;

public record GetNearestElevatorQuery : IRequest<IElevator>;