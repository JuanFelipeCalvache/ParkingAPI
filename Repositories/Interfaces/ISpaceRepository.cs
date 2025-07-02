using Parking.Models;

namespace Parking.Repositories.Interfaces
{
    public interface ISpaceRepository
    {
        Task<List<Space>> GetAllSpacesAsync();
        Task AddSpaceAsync(Space space);
        Task<Space?> GetSpaceById(int id);
        Task<Space?> GetAvailableSpaceByIdAsync(int spaceId);
        Task UpdateSpaceAsync(Space space);


    }
}
