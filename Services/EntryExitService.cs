using Parking.Data;
using Parking.DTOs;
using Microsoft.EntityFrameworkCore;
using Parking.Models;
using Parking.interfaces;

namespace Parking.Services
{
    public class EntryExitService : IEntryExitService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly ITariffService _tariffService;

        public EntryExitService(AppDbContext context, IConfiguration config, ITariffService tariffService)
        {
            _context = context;
            _config = config;
            _tariffService = tariffService;
        }


        public async Task<EntryExitResponseDTO> RegisterEntryAsync(EntryDTO entryDTO,VehicleDTO vehicleDTO )
        {
            var normalizedPlate = vehicleDTO.Plate.Trim().ToUpper();

            //Does the Vehicle exist ?
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.NumberPlate == normalizedPlate);
            if (vehicle == null)
            {
                vehicle = new Vehicle
                {

                    NumberPlate = vehicleDTO.Plate,
                    Type = vehicleDTO.Type,
                    Owner = vehicleDTO.Owner
                };

                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
            }


            //Is space empty
            var space = await _context.Spaces.FirstOrDefaultAsync(s => s.Id == entryDTO.SpaceId && !s.IsOccupied);
            if (space ==null)
            {
                return new EntryExitResponseDTO { Success = false, Message = "Space is not available. " };
            }


            var entryExit = new EntryExit
            {
                VehicleId = vehicle.Id,
                SpaceId = entryDTO.SpaceId,
                EntryTime = DateTime.Now,
                FeeToPaid = 0m
            };

            //Mark space like bussy
            space.IsOccupied = true;
            space.VehicleId = vehicle.Id;

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


            // Register Exit
            entryExit.ExitTime = exitDTO.ExitTime;

            var tariff = await _context.Tariffs.FirstOrDefaultAsync();

            if (tariff == null)
            {
                return new EntryExitResponseDTO { Success = false, Message = "Tariff configuration not found." };
            }

            //Total time in
            entryExit.FeeToPaid = _tariffService.CalculateFee(entryExit, tariff.RatePerHour);

            // Liberar espacio
            var space = await _context.Spaces.FirstOrDefaultAsync(s => s.Id == entryExit.SpaceId);
            if (space != null)
            {
                space.IsOccupied = false;
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
                AmountToPay = entryExit.FeeToPaid ?? 0,
                Success = true
            };
        }


        public async Task<List<EntryExitResponseDTO>> GetAllEntriesExitsAsync()
        {
            var entriesExits = await _context.EntryExits
                .Include(e => e.Vehicle)
                .Include(e => e.Space)
                .ToListAsync();

            var tariff = await _context.Tariffs.FirstOrDefaultAsync();

            if (tariff == null)
            {
                return new List<EntryExitResponseDTO>
                {
                    new EntryExitResponseDTO {Success = false, Message = "Tariff configuration not found"}
                };
            }


            var responseTask=  entriesExits.Select(async entryExit =>
            {

                var fee = _tariffService.CalculateFee(entryExit, tariff.RatePerHour);
                entryExit.FeeToPaid = fee;


                return new EntryExitResponseDTO
                {
                    Id = entryExit.Id,
                    VehiclePlate = entryExit.Vehicle.NumberPlate,
                    SpaceCode = entryExit.Space.Id.ToString(),
                    EntryTime = entryExit.EntryTime,
                    ExitTime = entryExit.ExitTime,
                    AmountToPay = fee,
                    Success = true
                };
             });

            var responelist = await Task.WhenAll(responseTask);

            return responelist.ToList();
        }


        public async Task<List<EntryExitResponseDTO>> GetEntrysInParking() {

            var vehiclesInParking = await _context.EntryExits
                .Include(v => v.Vehicle)
                .Include(v => v.Space)
                .Where(v => v.ExitTime == null)
                .ToListAsync();


            var tariff = await _context.Tariffs.FirstOrDefaultAsync();

            if (tariff == null)
            {
                return new List<EntryExitResponseDTO>
                {
                    new EntryExitResponseDTO {Success = false, Message = "Tariff configuration not found"}
                };
            }

            var result =  vehiclesInParking.Select(entry => 
            {
                var fee = _tariffService.CalculateFee(entry, tariff.RatePerHour);
                entry.FeeToPaid = fee;

                return new EntryExitResponseDTO
                {
                    Id = entry.Id,
                    VehiclePlate = entry.Vehicle.NumberPlate,
                    SpaceCode = entry.Space.Id.ToString(),
                    EntryTime = entry.EntryTime,
                    ExitTime = entry.ExitTime,
                    AmountToPay = fee
                };
            }).ToList();

            return result;

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


        public async Task<bool> DeleteEntryExitAsync(int id)
        {
            var record = await _context.EntryExits.FirstOrDefaultAsync(e => e.Id == id);

            if(record == null)
            {
                return false;
            }

            _context.EntryExits.Remove(record);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
