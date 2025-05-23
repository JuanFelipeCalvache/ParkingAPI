using Microsoft.AspNetCore.Mvc;
using Parking.DTOs;
using Parking.interfaces;
using Parking.Services;

namespace Parking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TariffController : ControllerBase
    {
        private readonly ITariffService _tariffService;
        
        public TariffController(ITariffService tariffService)
        {
            _tariffService = tariffService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TariffDTO>>> GetTariffs()
        {
            try
            {
                var tariffs = await _tariffService.GetAllAsync();
                return Ok(tariffs);
            }
            catch (Exception ex) 
            {
                return BadRequest($"Error: {ex.Message}");
            } 
        }

        [HttpGet("{vehicleType}")]
        public async Task<ActionResult<TariffDTO>> GetTariff(string vehicleType)
        {
            try
            {
                var tariff = await _tariffService.GetTariffAsync(vehicleType);
                return Ok(tariff);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddTariff([FromBody] TariffDTO dto)
        {
            await _tariffService.AddTariffAsync(dto);
            return Ok(new {message = "Tariff Added successfully"});
        }

        [HttpPut("tariff/change-infor")]
        public async Task<IActionResult> ChangeInfoTariff(TariffDTO dto)
        {
            if(dto == null)
            {
                return null;
            }

            var success = await _tariffService.UpdateTariffAsync(dto);

            if(!success)
            {
                return BadRequest("Invalid request");
            }

            return Ok("Satisfactorys changes");

        }

        [HttpDelete("tariff-delete/{id}")]
        public async Task<IActionResult> DeleteTariff(int id)
        {
            var success = await _tariffService.DeleteTariff(id);

            if(!success)
            {
                return BadRequest("Tariff not found");
            }

            return Ok("Record Deleted Succesfuly");
        }
    }
}
