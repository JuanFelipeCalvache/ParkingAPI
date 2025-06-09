using Parking.DTOs;

namespace Parking.Services.interfaces
{
    public interface IVehicleService
    {
        Task<List<VehicleDTO>> GetAllVehiclesAsync();

        Task<bool> DeleteVehicleAsync(int id);

    }
}
