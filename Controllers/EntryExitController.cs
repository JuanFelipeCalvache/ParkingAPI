﻿using Microsoft.AspNetCore.Mvc;
using Parking.DTOs;
using Parking.Services;
using Parking.Services.interfaces;


namespace Parking.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class EntryExitController : ControllerBase
    {
        private readonly IEntryExitService _entryExitService;

        public EntryExitController(IEntryExitService entryExitService)
        {
            _entryExitService = entryExitService;
        }

        [HttpPost("entry")]
        public async Task<IActionResult> RegisterEntry([FromBody] EntryRequestDTO request)
        {
            if (request == null || request.EntryDTO == null || request.VehicleDTO == null )
            {
                return BadRequest("Invalid data.");
            }

            var result = await _entryExitService.RegisterEntryAsync(request.EntryDTO, request.VehicleDTO);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }


        [HttpPost("exit/{vehiclePlate}")]
        public async Task<IActionResult> RegisterExit(string vehiclePlate, [FromBody]  ExitDTO exitDTO)
        {
            if(string.IsNullOrWhiteSpace(vehiclePlate) || exitDTO== null )
            {
                return BadRequest("Invalid data");
            }

            var result = await _entryExitService.RegisterExitByPlateAsync(vehiclePlate, exitDTO);
           
            if(!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }


        [HttpGet]
        public async Task<ActionResult<List<EntryExitResponseDTO>>> GetEntryExitsRegisters()
        {
            try
            {
                var entryExits = await _entryExitService.GetAllEntriesExitsAsync();
                return Ok(entryExits);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("entrysInParking")]
        public async Task<ActionResult<List<EntryExitResponseDTO>>> GetEntrysRegisters()
        {
            try
            {
                var entrys = await _entryExitService.GetEntrysInParking();
                return Ok(entrys);
            }
            catch (Exception exEntrys)
            {
                return BadRequest($"Error: {exEntrys.Message}");
            }
        }


        [HttpDelete("entry-exit/{id}")]
        public async Task<IActionResult> DeleteEntryExit(int id)
        {
            var success = await _entryExitService.DeleteEntryExitAsync(id);

            if(!success)
            {
                return BadRequest("Record not found");
            }

            return Ok("Record deleted successfully");
        }


    }
}
