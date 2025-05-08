using Parking.Data;
using Parking.DTOs;
using Microsoft.EntityFrameworkCore;
using Parking.Models;

namespace Parking.Services
{
    public class EntryExitService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public EntryExitService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<EntryExitResponseDTO> RegisterEntryAsync(EntryDTO entryDTO,VehicleDTO vehicleDTO )
        {
            //Does the Vehicle exist ?
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == entryDTO.VehicleId);
            if (vehicle == null)
            {
                vehicle = new Vehicle
                {
                    Id = vehicleDTO.Id,
                    NumberPlate = vehicleDTO.Plate,
                    Type = vehicleDTO.Type,
                    Owner = vehicleDTO.Owner

                };

                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
            }


            //Is space empty
            var space = await _context.Spaces.FirstOrDefaultAsync(s => s.Id == entryDTO.SpaceId && !s.IsOcuppied);
            if (space ==null)
            {
                return new EntryExitResponseDTO { Success = false, Message = "Space is not available. " };
            }


            var entryExit = new EntryExit
            {
                VehicleId = vehicle.Id,
                SpaceId = entryDTO.SpaceId,
                EntryTime = DateTime.UtcNow,
                
            };

            //Mark space like bussy
            space.IsOcuppied = true;

            _context.EntryExits.Add(entryExit);
            _context.Spaces.Update(space);
            await _context.SaveChangesAsync();

            return new EntryExitResponseDTO
            {
                Id = entryExit.Id,
                VehiclePlate = vehicle.NumberPlate,
                SpaceCode = space.Id.ToString(),
                EntryTime = entryExit.EntryTime

            };

        }


        public async Task<EntryExitResponseDTO> RegisterExitByPlateAsync(string vehiclePlate, ExitDTO exitDTO)
        {
            // Buscar entrada activa (sin ExitTime) del vehículo
            var entryExit = await _context.EntryExits
                .Include(e => e.Vehicle)
                .Where(e => e.Vehicle.NumberPlate == vehiclePlate && e.ExitTime == null)
                .OrderByDescending(e => e.EntryTime)
                .FirstOrDefaultAsync();

            if (entryExit == null)
            {
                return new EntryExitResponseDTO { Success = false, Message = "Active entry not found for this vehicle." };
            }

            // Registrar la salida
            entryExit.ExitTime = exitDTO.ExitTime;

            // Liberar espacio
            var space = await _context.Spaces.FirstOrDefaultAsync(s => s.Id == entryExit.SpaceId);
            if (space != null)
            {
                space.IsOcuppied = false;
                _context.Spaces.Update(space);
            }

            _context.EntryExits.Update(entryExit);
            await _context.SaveChangesAsync();

            return new EntryExitResponseDTO
            {
                Id = entryExit.Id,
                VehiclePlate = entryExit.Vehicle.NumberPlate,
                SpaceCode = space?.Id.ToString(),
                EntryTime = entryExit.EntryTime,
                ExitTime = entryExit.ExitTime,
                Success = true
            };
        }



        public async Task<List<EntryExitResponseDTO>> GetAllEntriesExitsAsync()
        {
            var entriesExits = await _context.EntryExits
                .Include(e => e.Vehicle)
                .Include(e => e.Space)
                .ToListAsync();

            return entriesExits.Select(entryExit => new EntryExitResponseDTO
            {
                Id = entryExit.Id,
                VehiclePlate = entryExit.Vehicle.NumberPlate,
                SpaceCode = entryExit.Space.Id.ToString(),
                EntryTime = entryExit.EntryTime,
                ExitTime = entryExit.ExitTime,
            }).ToList();
        }


        public async Task<List<EntryExitResponseDTO>> GetEntriesExitsByVehicleAsync(int vehicleId)
        {
            var entriesExits = await _context.EntryExits
                .Where(e => e.VehicleId == vehicleId)
                .Include(e => e.Vehicle)
                .Include(e => e.Space)
                .ToListAsync();

            return entriesExits.Select(entryExit => new EntryExitResponseDTO
            {
                Id = entryExit.Id,
                VehiclePlate = entryExit.Vehicle.NumberPlate,
                SpaceCode = entryExit.Space.Id.ToString(),
                EntryTime = entryExit.EntryTime,
                ExitTime = entryExit.ExitTime,
            }).ToList();
        }



    }
}
