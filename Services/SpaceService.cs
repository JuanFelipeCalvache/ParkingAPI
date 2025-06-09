using Microsoft.EntityFrameworkCore;
using Parking.Data;
using Parking.DTOs;
using Parking.Models;
using Parking.Repositories.Interfaces;
using Parking.Services.interfaces;
using System.Runtime.InteropServices;

namespace Parking.Services
{
    public class SpaceService : ISpaceService
    {
        private readonly ISpaceRepository _spaceRepo;

        public SpaceService(ISpaceRepository spaceRepo)
        {
            _spaceRepo = spaceRepo;

        }

        public async Task<List<SpaceDTO>> GetAllSpacesAsync()
        {
            var spaces = await _spaceRepo.GetAllSpacesAsync();


            return spaces.Select(s => new SpaceDTO
            {
                Id = s.Id,
                IsOccupied = s.IsOccupied,
                VehicleId = s.VehicleId
            }).ToList();
        }

        public async Task AddSpace(SpaceDTO spaceDTO)
        {
            var space = new Space
            {
                IsOccupied = spaceDTO.IsOccupied,
                VehicleId = spaceDTO.VehicleId
            };

            await _spaceRepo.AddSpaceAsync(space);
        }

        public async Task<bool> ChangeStateSpace(SpaceDTO spaceDto)
        {
            var space = await _spaceRepo.GetSpaceById(spaceDto.Id);

            if (space == null) return false;

            space.IsOccupied = spaceDto.IsOccupied;
            space.VehicleId = spaceDto.VehicleId;

            await _spaceRepo.UpdateSpaceAsync(space);

            return true;
                
        }
            

    }
}
