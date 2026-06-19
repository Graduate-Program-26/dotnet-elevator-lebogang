using MediatR;

public record BoardPassengersCommand(List<Passenger> WaitingPassangers, IElevator Elevator) : IRequest<Unit>;