using Parking.Data;

namespace Parking.Services
{
    public class SpaceService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public SpaceService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

        }
    }
}
