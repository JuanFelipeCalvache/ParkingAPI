using Parking.Models;

namespace Parking.Repositories.Interfaces
{
    public interface ITariffRepository
    {
        Task<List<Tariff>> GetAllTariffs();
        Task<Tariff?> GetTariffByVehicle(string vehicle);
        Task<Tariff?> GetTariffById(int id);
        Task AddTariffAsync(Tariff tariff);
        Task<bool> UpdateTariffAsync(Tariff tariff);
        Task<bool> DeleteTariffAsync(int id);
    }
}
