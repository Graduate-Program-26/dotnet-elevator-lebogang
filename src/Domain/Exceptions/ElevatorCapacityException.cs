public  class ElevatorCapacityException : Exception
{
    public ElevatorCapacityException(int maxCapacity): base($"Elevator capacity has exceeded the threshold {maxCapacity}")
    {
     
    }
}