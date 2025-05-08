using Microsoft.AspNetCore.Mvc;
using Parking.DTOs;
using Parking.Services;

namespace Parking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TariffController : Controller
    {
        private readonly TariffService _tariffService;
        
        public TariffController(TariffService tariffService)
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

        [HttpPost]
        public async Task<IActionResult> AddTariff([FromBody] TariffDTO dto)
        {
            await _tariffService.AddTariffAsync(dto);
            return Ok(new {message = "Tariff Added successfully"});
        }

    }
}
