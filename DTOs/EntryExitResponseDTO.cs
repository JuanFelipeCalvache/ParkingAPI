namespace Parking.DTOs
{
    public class EntryExitResponseDTO
    {
        public int Id { get; set; }
        public string VehiclePlate { get; set; }
        public string SpaceCode { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public TimeSpan? Duration => ExitTime.HasValue ? ExitTime.Value - EntryTime : null;
    }
}
