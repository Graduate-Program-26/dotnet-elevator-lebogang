using MediatR;

public record GetSystemStateQuery : IRequest<SystemStatusDto>;