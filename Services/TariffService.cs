using Microsoft.EntityFrameworkCore;
using Parking.Data;
using Parking.DTOs;
using Parking.Models;

namespace Parking.Services
{
    public class TariffService
    {
        private readonly AppDbContext _context;

        public TariffService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TariffDTO>> GetAllAsync()
        {
            var tariffs = await _context.Tariffs
                .Select(t => new TariffDTO
                {
                    VehicleType = t.VehicleType,
                    RatePerHour = t.RatePerHour
                })
                .ToListAsync();
            return tariffs;
        }

        public async Task<Tariff> GetByVehicleTypeAsync(string vehicleType)
        {
            return await _context.Tariffs.FirstOrDefaultAsync(t => t.VehicleType == vehicleType);
        }

        public async Task AddTariffAsync(TariffDTO tariffDTO)
        {
            var tariff = new Tariff
            {
                VehicleType = tariffDTO.VehicleType,
                RatePerHour = tariffDTO.RatePerHour
            };

            _context.Tariffs.Add(tariff);
            await _context.SaveChangesAsync();

        }
    }
}
