using MediatR;

public record ToggleSimCommand(bool Enabled) : IRequest<Unit>;