public class PassengerGeneration
{

    /// <summary>
    ///  whether passengers should be generated automatcially or not
    /// </summary>
    public bool AutoGen {get; set;} = true;

    /// <summary>
    /// How often new passengers are generated
    /// </summary>
    public int GenInterval {get; set;} = 3000;

    public int MaxPassengersPerFloor {get; set;} = 5;
    
    /// <summary>Maximum number of floors that receive passengers per generation tick.</summary>
    public int MaxFloorsPerTick { get; set; } = 2;

}