using Microsoft.EntityFrameworkCore;
using Parking.Models;
using Parking.Data;
using Parking.Repositories.Interfaces;


namespace Parking.Repositories
{
    public class EntryExitRepository : IEntryExitRepository
    {
        private readonly AppDbContext _context;

        public EntryExitRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EntryExit?> GetActiveEntryByPlateAsync(string plate)
        {
            return await _context.EntryExits
                .Include(e => e.Vehicle)
                .Where(e => e.Vehicle.NumberPlate == plate && e.ExitTime == null)
                .OrderByDescending(e => e.EntryTime)
                .FirstOrDefaultAsync();

        }

        public async Task<List<EntryExit>> GetAllAsync()
        {
            return await _context.EntryExits
                .Include(e => e.Vehicle)
                .Include(e => e.Space)
                .ToListAsync();
        }


        public async Task<List<EntryExit>> GetAllActiveAsync()
        {
            return await _context.EntryExits
                .Include(e => e.Vehicle)
                .Include(e => e.Space)
                .Where(e => e.ExitTime == null)
                .ToListAsync();
        }

        public async Task<List<EntryExit>> GetByVehicleIdAsync(int vehicleId)
        {
            return await _context.EntryExits
                .Include(e => e.Vehicle)
                .Include(e => e.Space)
                .Where(e => e.VehicleId == vehicleId)
                .ToListAsync();
        }

        public async Task<EntryExit?> GetByIdAsync(int id)
        {
            return await _context.EntryExits.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task DeleteAsync(int id)
        {
            var entry = await _context.EntryExits.FirstOrDefaultAsync(e => e.Id == id);
            if (entry != null)
            {
                _context.EntryExits.Remove(entry);
                await _context.SaveChangesAsync();
            };

        }

        public async Task AddAsync(EntryExit entryExit)
        {
            _context.EntryExits.Add(entryExit);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EntryExit entryExit)
        {
            _context.EntryExits.Update(entryExit);
            await _context.SaveChangesAsync();
        }


    }    
    
}
