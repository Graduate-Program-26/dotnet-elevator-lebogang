using MediatR;

public record MoveElevatorCommand(IElevator Elevator, int FloorNumber) : IRequest<Unit>;