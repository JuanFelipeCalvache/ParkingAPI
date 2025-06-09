using Parking.DTOs;

namespace Parking.Services.interfaces
{
    public interface ISpaceService
    {
        Task<List<SpaceDTO>> GetAllSpacesAsync();
        Task AddSpace(SpaceDTO spaceDTO);
        Task<bool> ChangeStateSpace(SpaceDTO spaceDTO);
    }
}
