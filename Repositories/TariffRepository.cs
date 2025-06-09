using Microsoft.EntityFrameworkCore;
using Parking.Models;
using Parking.Data;
using Parking.Repositories.Interfaces;


namespace Parking.Repositories
{
    public class TariffRepository : ITariffRepository
    {
        private readonly AppDbContext _context;

        public TariffRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tariff>> GetAllTariffs()
        {
            return await _context.Tariffs.ToListAsync();
        }

        public async Task<Tariff?> GetTariffByVehicle(string vehicle)
        {
            return await _context.Tariffs.FirstOrDefaultAsync(t => t.VehicleType == vehicle);
        }

        public async Task<Tariff?> GetTariffById(int id)
        {
            return await _context.Tariffs.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddTariffAsync(Tariff tariff)
        {
            await _context.Tariffs.AddAsync(tariff);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateTariffAsyn(Tariff tariff)
        {
            var existing = await _context.Tariffs.FirstOrDefaultAsync(t => t.Id == tariff.Id);

            if (existing == null) return false;

            existing.VehicleType = tariff.VehicleType;
            existing.RatePerHour = tariff.RatePerHour; ;

            _context.Tariffs.Update(existing);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTariffAsync(int id)
        {
            var tariff = await _context.Tariffs.FirstOrDefaultAsync(t => t.Id == id);

            if (tariff == null) return false;

            _context.Tariffs.Remove(tariff);
            await _context.SaveChangesAsync();

            return true;
        }



    }
}
