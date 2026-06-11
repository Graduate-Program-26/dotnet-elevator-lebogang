using MediatR;

public record DisambarkPassengersCommand(int FloorNumber, IElevator Elevator) : IRequest<Unit>;