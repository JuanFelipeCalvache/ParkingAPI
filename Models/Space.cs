namespace Parking.Models
{
    public class Space
    {
        public int Id { get; set; }
        public bool IsOccupied { get; set; }

        public int? VehicleId { get; set; }
        public Vehicle? vehicle { get; set; }
    }
}

