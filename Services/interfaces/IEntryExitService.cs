﻿using Parking.DTOs;

namespace Parking.Services.interfaces
{
    public interface IEntryExitService
    {
        Task<EntryExitResponseDTO> RegisterEntryAsync(EntryDTO entryDTO, VehicleDTO vehicleDto);

        Task<EntryExitResponseDTO> RegisterExitByPlateAsync(string vehiclePlate, ExitDTO exitDTO);

        Task<List<EntryExitResponseDTO>> GetAllEntriesExitsAsync();

        Task<List<EntryExitResponseDTO>> GetEntrysInParking();

        Task<List<EntryExitResponseDTO>> GetEntriesExitsByVehicleAsync(int id);

        Task<bool> DeleteEntryExitAsync(int id);

    }
}
