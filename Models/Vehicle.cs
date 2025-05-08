namespace Parking.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string NumberPlate {  get; set; }
        public string Type { get; set; }
        public string Owner { get; set; }

        public ICollection<EntryExit> entriesExits { get; set; }
    }
}
