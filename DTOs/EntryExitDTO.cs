namespace Parking.DTOs
{
    public class EntryExitDTO
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int SpaceId { get; set; }
        public DateTime EntryTime { get ; set; }
        public DateTime ExitTime { get; set; }

    }
}
