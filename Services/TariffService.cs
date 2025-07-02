using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Parking.Data;
using Parking.DTOs;
using Parking.Models;
using Parking.Repositories;
using Parking.Repositories.Interfaces;
using Parking.Services.interfaces;

namespace Parking.Services
{
    public class TariffService : ITariffService
    {
        private readonly ITariffRepository _tariffRepo;

        public TariffService(ITariffRepository tariffRepo)
        {
            _tariffRepo = tariffRepo;
        }

        public async Task<List<TariffDTO>> GetAllAsync()
        {
            var tariffs = await _tariffRepo.GetAllTariffs();

            return tariffs.Select(t => new TariffDTO
            {
                VehicleType = t.VehicleType,
                RatePerHour = t.RatePerHour
            }).ToList();
        }

        public async Task<TariffDTO> GetTariffAsync(string vehicle)
        {
            var tariff = await _tariffRepo.GetTariffByVehicle(vehicle);

            if (tariff == null) return null;


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

            await _tariffRepo.AddTariffAsync(tariff);

        }

        public async Task<bool> UpdateTariffAsync(TariffDTO tariffDTO)
        {
            var tariff = await _tariffRepo.GetTariffById(tariffDTO.id);
            
            if (tariff == null) return false;

            tariff.VehicleType = tariffDTO.VehicleType;
            tariff.RatePerHour = tariffDTO.RatePerHour;

            await _tariffRepo.UpdateTariffAsync(tariff);

            return true;

        }


        public async Task<bool> DeleteTariff(int id)
        {
            var tariff = await _tariffRepo.GetTariffById(id);

            if(tariff == null) return false;

            await _tariffRepo.DeleteTariffAsync(tariff.Id);

            return true;

        }

        public decimal CalculateFee(EntryExit entryExit, decimal ratePerHour)
        {
            
            var totalHours = (decimal)Math.Ceiling((DateTime.Now - entryExit.EntryTime).TotalHours);
            return totalHours * ratePerHour;
        }

    }
}
