using Microsoft.EntityFrameworkCore;
using Parking.Data;
using Parking.Models;
using Parking.Repositories.Interfaces;

namespace Parking.Repositories
{
    public class SpaceRepository : ISpaceRepository
    {
        private readonly AppDbContext _context;

        public SpaceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Space>> GetAllSpacesAsync()
        {
            return await _context.Spaces.ToListAsync();
        }

        public async Task AddSpaceAsync(Space space)
        {
            await _context.Spaces.AddAsync(space);
            await _context.SaveChangesAsync();
        }

        public async Task<Space?> GetSpaceById(int id)
        {
            return await _context.Spaces.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateSpaceAsync(Space space)
        {
            _context.Spaces.Update(space);
            await  _context.SaveChangesAsync();
        }



    }
}
