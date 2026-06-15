using MediatR;

public record MoveElevatorCommand(IElevator Elevator) : IRequest<Unit>;