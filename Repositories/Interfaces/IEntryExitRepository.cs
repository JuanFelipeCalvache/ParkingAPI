using Parking.Models;

namespace Parking.Repositories.Interfaces
{
    public interface IEntryExitRepository
    {
        Task<EntryExit?> GetActiveEntryByPlateAsync(string plate);
        Task<List<EntryExit>> GetAllAsync();
        Task<List<EntryExit>> GetAllActiveAsync();
        Task<List<EntryExit>> GetByVehicleIdAsync(int vehicleId);
        Task<EntryExit?> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task AddAsync(EntryExit entryExit);
        Task UpdateAsync(EntryExit entryExit);
    }
}
