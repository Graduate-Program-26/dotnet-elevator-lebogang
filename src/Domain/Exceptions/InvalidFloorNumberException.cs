public class InvalidFloorNumberExecption : Exception
{
    public InvalidFloorNumberExecption(int floorNumber) :base($"Floor number {floorNumber} is invalid for building")
    {
        
    }
}