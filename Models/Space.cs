namespace Parking.Models
{
    public class Space
    {
        public int Id { get; set; }
        public bool IsOcuppied { get; set; }

        public int? VehicleId { get; set; }
        public Vehicle vehicle { get; set; }
    }
}

