public class Floor
{

   private readonly List<Passenger> _waitingPassengers = new();

   public IReadOnlyList<Passenger> WaitingPassengers => _waitingPassengers.AsReadOnly();
   public int FloorNumber {get; } = 0;
   
   public Floor(int floorNumber )
   {
      FloorNumber = floorNumber;
   }

   public void MapPassengerToElevator()
   {
      // if there is a valid floor request and elevator is on that floor, open elevator and service the relevant requests 

   }

   public void AddWaitingPassenger(Passenger passenger)
   {
      ArgumentNullException.ThrowIfNull(passenger);
      _waitingPassengers.Add(passenger);
   }

   public void RemoveWaitingPassenger(Passenger passenger)
   {
      _waitingPassengers.Remove(passenger);
   }
   
 }