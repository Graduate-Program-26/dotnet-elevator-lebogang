using MediatR;

public record GetSystemStateQuery : IRequest<SystemState>;