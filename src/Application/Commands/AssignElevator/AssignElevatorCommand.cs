using MediatR;

public record AssignElevatorCommand(IElevator Elevator, Passenger Passenger) : IRequest<Unit>;