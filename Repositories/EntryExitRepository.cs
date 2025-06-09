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
                .Include(e => e.SpaceId)
                .Where(e => e.VehicleId == vehicleId)
                .ToListAsync();
        }

        public async Task<EntryExit?> GetByIdAsync(int id)
        {
            return await _context.EntryExits.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.EntryExits.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return false;

            _context.EntryExits.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
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
