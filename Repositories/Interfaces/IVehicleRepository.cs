using Parking.Models;

namespace Parking.Repositories.Interfaces
{
    public interface IVehicleRepository
    {
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle?> GetByIdAsync(int id);
        Task DeleteAsync(Vehicle vehicle);
    }
}
