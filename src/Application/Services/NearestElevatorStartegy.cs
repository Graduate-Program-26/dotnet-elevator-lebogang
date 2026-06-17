public class NearestElevatorStrategy : IDispatchController
{
    /// <summary>
    //   1. Is the elevator available? (not full, not in maintenance)
    // 2. Will the elevator pass currentFloor at all, given its current queue?
    // 3. If yes — when it passes currentFloor, which direction will it be going?
    // 4. Does that direction match requestedDirection?
    // 5. If match  → strong candidate, score by distance
    // 6. If no match (wrong direction, or idle) → weaker candidate, 
    //    only chosen if no matching elevator exists
    /// </summary>
    /// <returns>Best candidate elevator </returns>

    public IElevator? FindBestElevator(List<IElevator> elevators, int currentFloor, ElevatorDirection requestedDirection)
    {
        
        
        if (elevators.Count == 0) return null;

        var avialableElevators = elevators
                                .Where(elevator => elevator.State != ElevatorState.OutOfService)
                                .Where(elevator => elevator.HasCapacity()).ToList();

        if (avialableElevators.Count == 0) return null;

        var matchingDirectionElevators = avialableElevators
                                .Where(elevator => DirectionMatches(elevator, requestedDirection))
                                .Where(elevator => WillPassFloor(elevator, currentFloor)).ToList();

        if (matchingDirectionElevators.Count > 0) return SortElevatorsByDistance(matchingDirectionElevators, currentFloor).First();

        return SortElevatorsByDistance(avialableElevators, currentFloor).First();
    }

    private static List<IElevator> SortElevatorsByDistance(List<IElevator> elevators, int currentFloor)
    {
        return elevators.OrderBy(elevator => Math.Abs(elevator.CurrentFloor - currentFloor)).ToList();
    }

    private static bool WillPassFloor(IElevator elevator, int targetFloor)
    {
        if (elevator.RequestedDirection == ElevatorDirection.Stationary) return true;

        if (elevator.RequestedDirection == ElevatorDirection.Up)
        {
            return elevator.FloorRequests.Any(f => f >= targetFloor) && targetFloor >= elevator.CurrentFloor;
        }

        if (elevator.RequestedDirection == ElevatorDirection.Down)
        {
            return elevator.FloorRequests.Any(f => f <= targetFloor) && targetFloor <= elevator.CurrentFloor;
        }

        return false; // stationary — won't pass anything
    }

    /// <summary>
    /// Returns true if the elevator can serve the passenger's requested direction.
    /// A stationary elevator has no committed direction and can serve any request —
    /// it is treated as a wildcard match and preferred over elevators heading
    /// the wrong way.
    /// </summary>
    private static bool DirectionMatches(IElevator elevator,ElevatorDirection passengerDirection)
    {
        // stationary = idle, no committed direction, can go either way
        if (elevator.RequestedDirection == ElevatorDirection.Stationary)
            return true;

        return elevator.RequestedDirection == passengerDirection;
    }
}