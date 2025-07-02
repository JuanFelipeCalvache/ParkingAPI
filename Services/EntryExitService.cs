using Parking.Data;
using Parking.DTOs;
using Microsoft.EntityFrameworkCore;
using Parking.Models;
using Parking.Services.interfaces;
using Parking.Repositories;
using System.Diagnostics;
using Parking.Repositories.Interfaces;

namespace Parking.Services
{
    public class EntryExitService : IEntryExitService
    {
        private readonly IEntryExitRepository _entryExitRepo;
        private readonly ITariffService _tariffService;
        private readonly ITariffRepository _tariffRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly ISpaceRepository _spaceRepo;

        public EntryExitService(IEntryExitRepository entryExitRepo, ITariffService tariffService, IVehicleRepository vehicleRepo, ISpaceRepository spaceRepo, ITariffRepository tariffRepo)
        {
            _entryExitRepo = entryExitRepo;
            _tariffService = tariffService;
            _tariffRepo = tariffRepo;
            _vehicleRepo = vehicleRepo;
            _spaceRepo = spaceRepo;
        }


        public async Task<EntryExitResponseDTO> RegisterEntryAsync(EntryDTO entryDTO,VehicleDTO vehicleDTO )
        {
            var normalizedPlate = vehicleDTO.Plate.Trim().ToUpper();

            //Does the Vehicle exist ?
            var vehicle = await _vehicleRepo.GetVehicleByPlate(normalizedPlate);
            if (vehicle == null)
            {
                vehicle = new Vehicle
                {

                    NumberPlate = normalizedPlate,
                    Type = vehicleDTO.Type,
                    Owner = vehicleDTO.Owner
                };

                await _vehicleRepo.AddVehicleAsync(vehicle);
                
            }

            var activeEntry = await _entryExitRepo.GetActiveEntryByPlateAsync(normalizedPlate);

            if(activeEntry != null)
            {
                return new EntryExitResponseDTO
                {
                    Success = false,
                    Message = "The vehicle is already inside the parking lot"
                };
            }


            //Is space empty
            var space = await _spaceRepo.GetAvailableSpaceByIdAsync(entryDTO.SpaceId);            
            if (space == null)
            {
                return new EntryExitResponseDTO { Success = false, Message = "Space is not available." };
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

            await _entryExitRepo.AddAsync(entryExit);
            await _spaceRepo.UpdateSpaceAsync(space);
            

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
            var activeEntry = await _entryExitRepo.GetActiveEntryByPlateAsync(vehiclePlate);

            if (activeEntry == null)
            {
                return new EntryExitResponseDTO { Success = false, Message = "Active entry not found for this vehicle." };
            }

            // Registrar salida
            activeEntry.ExitTime = exitDTO.ExitTime;
            
            var rate = exitDTO.TariffDTO?.RatePerHour ?? 0;
            if (rate <= 0)
            {
                return new EntryExitResponseDTO { Success = false, Message = "Invalid or missing tariff information." };
            }

            // Calcular tarifa
            activeEntry.FeeToPaid = _tariffService.CalculateFee(activeEntry, rate);

            var space = await _spaceRepo.GetSpaceById(activeEntry.SpaceId.Value);

            // Liberar espacio
                
            if (space != null)
            {
                space.IsOccupied = false;
                space.VehicleId = null;
                await _spaceRepo.UpdateSpaceAsync(space);
            }


            await _entryExitRepo.UpdateAsync(activeEntry);

            return new EntryExitResponseDTO
            {
                Id = activeEntry.Id,
                VehiclePlate = activeEntry.Vehicle.NumberPlate,
                SpaceCode = space?.Id.ToString(),
                EntryTime = activeEntry.EntryTime,
                ExitTime = activeEntry.ExitTime,
                AmountToPay = activeEntry.FeeToPaid ?? 0,
                Success = true
            };
        }



        public async Task<List<EntryExitResponseDTO>> GetAllEntriesExitsAsync()
        {
            var entriesExits =  await _entryExitRepo.GetAllAsync();

            var tariff = (await _tariffRepo.GetAllTariffs())?.FirstOrDefault();

            if (tariff == null)
            {
                return new List<EntryExitResponseDTO>
                {
                    new EntryExitResponseDTO {Success = false, Message = "Tariff configuration not found"}
                };
            }

            var responseTask =  entriesExits.Select(entryExit =>
            {
                decimal fee;

                if (entryExit.ExitTime == null)
                {
                    fee = _tariffService.CalculateFee(entryExit, tariff.RatePerHour);
                    entryExit.FeeToPaid = fee;
                }
                else
                {
                    fee = entryExit.FeeToPaid ?? 0;
                }


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

            var responelist = responseTask.ToList();

            return responelist.ToList();
        }


        public async Task<List<EntryExitResponseDTO>> GetEntrysInParking() {

            var vehiclesInParking = await _entryExitRepo.GetAllActiveAsync();

            var tariff = (await _tariffRepo.GetAllTariffs())?.FirstOrDefault();

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
            var entriesExits = await _entryExitRepo.GetByVehicleIdAsync(vehicleId);
            
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
            var record = await _entryExitRepo.GetByIdAsync(id);

            if(record == null) return false;

            await _entryExitRepo.DeleteAsync(record.Id);

            return true;
        }

    }
}
