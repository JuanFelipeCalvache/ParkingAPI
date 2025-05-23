namespace Parking.Models
{
    public class EntryExit
    {
        public int Id { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        public int? SpaceId { get; set; }
        public Space Space { get; set; }


        public decimal? FeeToPaid { get; set; }
    }
}
