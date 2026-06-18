using MediatR;

public record RegisterPassengerCommand(int SourceFloor,int DestinationFloor) : IRequest<Unit>;