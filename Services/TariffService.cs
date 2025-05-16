using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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

        public async Task<TariffDTO> GetTariffAsync(string vehicle)
        {
            var tariff = await _context.Tariffs
                .FirstOrDefaultAsync(t => t.VehicleType == vehicle);

            if (tariff == null)
            {
                return null;
            }

            return new TariffDTO
            {
                VehicleType = tariff.VehicleType,
                RatePerHour = tariff.RatePerHour
            };
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

        public async Task<bool> UpdateTariffAsync(TariffDTO tariffDTO)
        {
            var tariff = await _context.Tariffs.FirstOrDefaultAsync(t => t.Id == tariffDTO.id);

            if (tariff == null)
            {
                return false;
            }

            tariff.VehicleType = tariffDTO.VehicleType;
            tariff.RatePerHour = tariffDTO.RatePerHour;

            _context.Tariffs.Update(tariff);
            await _context.SaveChangesAsync();

            return true;

        }


        public async Task<bool> DeleteTariff(int id)
        {
            var tariff = await _context.Tariffs.FirstOrDefaultAsync(t => t.Id == id);

            if(tariff == null)
            {
                return false;
            }

            _context.Tariffs.Remove(tariff);
            await _context.SaveChangesAsync();

            return true;

        }
    }
}
