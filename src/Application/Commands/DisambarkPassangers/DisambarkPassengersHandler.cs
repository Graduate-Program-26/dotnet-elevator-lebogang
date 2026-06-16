using MediatR;

public class DisambarkPassengersCommandHandler : IRequestHandler<DisambarkPassengersCommand, Unit>
{
      

    public DisambarkPassengersCommandHandler()
    {

    }

    public Task<Unit> Handle(DisambarkPassengersCommand command, CancellationToken token)
    {
   
        var disembarked = command.Elevator.DisembarkAtCurrentFloor();

        foreach (var passenger in disembarked)
        {
            passenger.Disembark();
        }
    
        return Unit.Task;
    }
}