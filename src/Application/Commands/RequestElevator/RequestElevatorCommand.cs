using MediatR;

public record RequestElevatorCommand(int FloorNumber,ElevatorDirection Direction ) : IRequest<Unit>;