using Parking.Models;

namespace Parking.Repositories.Interfaces
{
    public interface IVehicleRepository
    {
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle?> GetByIdAsync(int id);
        Task<Vehicle?> GetVehicleByPlate(string plate);
        Task AddVehicleAsync(Vehicle vehicle);
        Task DeleteAsync(Vehicle vehicle);
    }
}
