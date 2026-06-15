

using MediatR;

public class MoveElevatorCommandHandler : IRequestHandler<MoveElevatorCommand, Unit>
{


    public MoveElevatorCommandHandler()
    {
   
    }



    public Task<Unit> Handle(MoveElevatorCommand command, CancellationToken token)
    {
        if(command.Elevator.FloorRequests.Count == 0) return Unit.Task;
        
        command.Elevator.State = ElevatorState.Traveling;
        // set elevator direction
        command.Elevator.RequestedDirection = command.Elevator.CurrentFloor < command.FloorNumber ? ElevatorDirection.Up : ElevatorDirection.Down;

        command.Elevator.MoveOneFloor();

  

        return Unit.Task;
    }
}