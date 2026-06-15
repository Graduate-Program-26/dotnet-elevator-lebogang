using MediatR;

public class BoardPassengersCommandHandler : IRequestHandler<BoardPassengersCommand, Unit>
{
    

    public BoardPassengersCommandHandler()
    {
    
    }

    public Task<Unit> Handle(BoardPassengersCommand command, CancellationToken token)
    {
        List<Passenger> boardingPassengers = new();
        int spaceAvailabie = command.Elevator.MaxCapacity - command.Elevator.CurrentCapacity;

        foreach (var pasenger in command.WaitingPassangers.ToList())
        {
            if(spaceAvailabie == 0)
            {
                break;
            }
            
            if(pasenger.RequestedDirection == command.Elevator.RequestedDirection)
            {
               boardingPassengers.Add(pasenger);
               pasenger.Board();
               pasenger.Status = PassengerStatus.InTransit;
               command.WaitingPassangers.Remove(pasenger);
               spaceAvailabie--;
            }
        }


        command.Elevator.BoardPassengers(boardingPassengers);

        return Unit.Task;
    }
}