public record ElevatorStatusDto(
    Guid Id,
    string Label,          
    int CurrentFloor,
    ElevatorDirection Direction,
    ElevatorState State,
    int CurrentCapacity,
    int MaxCapacity,
    IReadOnlyList<int> FloorRequests
);
