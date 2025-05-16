using Microsoft.AspNetCore.Mvc;
using Parking.DTOs;
using Parking.Services;

namespace Parking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpaceController : Controller
    {
        private readonly SpaceService _spaceService;

        public SpaceController(SpaceService spaceService)
        {
            _spaceService = spaceService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SpaceDTO>>> GetSpaces()
        {
            try
            {
                var spaces = await _spaceService.GetAllSpacesAsync();
                return Ok(spaces);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSpace([FromBody] SpaceDTO dto)
        {
            await _spaceService.AddSpace(dto);
            return Ok(new { message = "Space added successfully" });
        }

        [HttpPut("space/change-state")]
        public async Task<IActionResult> ChangeSpaceState([FromBody] SpaceDTO dto)
        {
            if(dto == null)
            {
                return BadRequest("Invalid request");
            }

            var success = await _spaceService.ChangeStateSpace(dto);

            if(!success)
            {
                return NotFound("Space not found");
            }

            return Ok("space state updated");
        }

        


    }
}
