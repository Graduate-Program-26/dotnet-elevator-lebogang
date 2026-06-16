public record ElevatorStateDto( 
    Guid Id,
   
    int CurrentFloor,
    ElevatorDirection Direction,
    ElevatorState State,
    int CurrentCapacity,
    int MaxCapacity,
    IReadOnlyList<int> FloorRequests,
    IReadOnlyList<int> OnboardPassengerDestinations
);