public record SystemStatusDto(
    IReadOnlyList<ElevatorStatusDto> Elevators,
    IReadOnlyList<FloorStatusDto> Floors
);


