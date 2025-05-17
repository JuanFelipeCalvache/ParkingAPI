using Microsoft.AspNetCore.Mvc;
using Parking.DTOs;
using Parking.Services;

namespace Parking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : Controller
    {

        private readonly VehicleService _vehicleService;
        public VehicleController(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<ActionResult<List<VehicleDTO>>> GettAllVehicles()
        {
            try
            {
                var vehicles = await _vehicleService.GetAllVehiclesAsync();
                return Ok(vehicles);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpDelete("DeleteVehicle{id}")]
        public async Task<ActionResult> DeleteVehicle(int id)
        {
            var success = await _vehicleService.DeleteVehicleAsync(id);

            if (!success)
            {
                return BadRequest("Vehicle not found");
            }

            return Ok("Vehicle deleted successfully");

        }


    }
}
