namespace Parking.DTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public UserDTO User { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public DateTime Expiration { get; set; }
    }
}
