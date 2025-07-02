using Parking.DTOs;
using Parking.Models;
using Parking.Repositories.Interfaces;
using Parking.Services.interfaces;

namespace Parking.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;


        public VehicleService(IVehicleRepository vehicleRepo)
        {
            _vehicleRepository = vehicleRepo;

        }


        public async Task<List<VehicleDTO>> GetAllVehiclesAsync()
        {
            var vehicles = await _vehicleRepository.GetAllVehiclesAsync();
            
            return vehicles.Select(v => new VehicleDTO
            {
                Id = v.Id,
                Plate = v.NumberPlate,
                Type = v.Type,
                Owner = v.Owner,
            }).ToList();
        }


        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null) return false;

            await _vehicleRepository.DeleteAsync(vehicle);
            return true;
        }

    }
}
