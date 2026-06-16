public record ElevatorStatusDto(
    Guid Id,
         
    int CurrentFloor,
    ElevatorDirection Direction,
    ElevatorState State,
    int CurrentCapacity,
    int MaxCapacity,
    IReadOnlyList<int> FloorRequests
);
