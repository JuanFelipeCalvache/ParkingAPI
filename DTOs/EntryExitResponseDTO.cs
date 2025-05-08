namespace Parking.DTOs
{
    public class EntryExitResponseDTO
    {
        public int Id { get; set; }
        public string VehiclePlate { get; set; }
        public string SpaceCode { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public decimal AmountToPay { get; set; }

        public bool Success { get; set; } = true;
        public string Message { get; set; }
    }
}
