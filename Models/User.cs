namespace Parking.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Rol { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }

    }
}
