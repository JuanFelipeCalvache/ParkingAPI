using Parking.DTOs;
using Parking.Models;

namespace Parking.interfaces
{
    public interface ITariffService
    {
        Task<List<TariffDTO>> GetAllAsync();
        Task<TariffDTO> GetTariffAsync(string vehicle);
        Task AddTariffAsync (TariffDTO tariffDTO);
        Task<bool> UpdateTariffAsync(TariffDTO tariffDTO);
        Task<bool> DeleteTariff(int id);

        decimal CalculateFee(EntryExit entryExit, decimal ratePerHour);
    }
}
