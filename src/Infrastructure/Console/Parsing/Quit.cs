using MediatR;


// does not need handler, does not go to MediatR 
public record Quit() : IRequest<Unit>;