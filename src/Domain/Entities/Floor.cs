public class Floor
{

   private readonly List<Passenger> _waitingPassengers = new();

   public IReadOnlyList<Passenger> WaitingPassengers => _waitingPassengers.AsReadOnly();
   
   public bool HasWaitingPassengers {get
      {
         return WaitingPassengers.Count > 0;
      }
   }
   
   public int FloorNumber { get; private set; }

   public Floor(int floorNumber)
   {
      FloorNumber = floorNumber;
   }

   public void AddWaitingPassenger(Passenger passenger)
   {
      ArgumentNullException.ThrowIfNull(passenger);

      if (passenger.SourceFloor != FloorNumber)
      {
         throw new InvalidPassengerAddedToFloor($"Passenger with source floor {passenger.SourceFloor} " +
            $"cannot be added to floor {FloorNumber}.");
      }
      _waitingPassengers.Add(passenger);
   }

   public void RemoveWaitingPassenger(Passenger passenger)
   {
      _waitingPassengers.Remove(passenger);
   }

}