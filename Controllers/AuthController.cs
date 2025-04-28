using Microsoft.AspNetCore.Mvc;
using Parking.DTOs;
using Parking.Services;

namespace Parking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var user = await _authService.RegisterAsync(dto);
            if (user == null)
            {
                return BadRequest("El correo ya está en uso.");
            }

            var token = _authService.GenerateToken(user);
            var response = new AuthResponseDTO
            {
                Token = token,
                User = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.Name,
                    Rol = user.Rol,

                }
                
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _authService.ValidateUserAsync(dto);
            if (user == null)
            {
                return Unauthorized("Credenciales inválidas.");
            }

            var token = _authService.GenerateToken(user);
            var response = new AuthResponseDTO
            {
                Token = token,
                User = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.Name,
                    Rol = user.Rol,

                }
            };

            return Ok(response);
        }
    }
}
