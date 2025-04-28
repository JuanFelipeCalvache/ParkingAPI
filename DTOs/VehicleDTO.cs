namespace Parking.DTOs
{
    public class VehicleDTO
    {
        public int Id { get; set; }
        public string plate { get; set; }
        public string brand { get; set; }
        public string color { get; set; }
        public int UserId   { get; set; }
    }
}
