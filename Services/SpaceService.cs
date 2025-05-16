using Microsoft.EntityFrameworkCore;
using Parking.Data;
using Parking.DTOs;
using Parking.Models;
using System.Runtime.InteropServices;

namespace Parking.Services
{
    public class SpaceService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public SpaceService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

        }

        public async Task<List<SpaceDTO>> GetAllSpacesAsync()
        {
            var spaces = await _context.Spaces
                .Select(s => new SpaceDTO
                {
                    Id = s.Id,
                    IsOccupied = s.IsOccupied,
                    VehicleId = s.VehicleId
                })
                .ToListAsync();

            return spaces;
        }

        public async Task AddSpace(SpaceDTO spaceDTO)
        {
            var space = new Space
            {
                IsOccupied = spaceDTO.IsOccupied,
                VehicleId = spaceDTO.VehicleId
            };

            _context.Spaces.Add(space);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ChangeStateSpace(SpaceDTO spaceDto)
        {
            var space = await _context.Spaces.FirstOrDefaultAsync(s => s.Id == spaceDto.Id);

            if (space == null)
            {
                return false;
            }

            space.IsOccupied = spaceDto.IsOccupied;
            space.VehicleId = spaceDto.VehicleId;

            _context.Spaces.Update(space);
            await _context.SaveChangesAsync();


            return true;
                
        }
            

    }
}
