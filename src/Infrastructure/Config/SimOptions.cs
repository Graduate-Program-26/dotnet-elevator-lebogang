public class SimOptions
{
    public int TickInterval {get; set;} = 500;

    public int TotalFloors {get; set;}  = 11;

    public int TotalElevators {get; set;} = 4;

    public PassengerGeneration PassengerGenerationEngine {get; set;} = new();
}