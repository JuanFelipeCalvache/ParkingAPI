using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Parking.Models;
using Parking.DTOs;
using Parking.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;


namespace Parking.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

        }

        public async Task<User?> RegisterAsync(RegisterDTO registerDto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Email == registerDto.Email);
            if (userExists) return null;

            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Name = registerDto.UserName,
                Rol = registerDto.Rol

            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();


            return user;

        }


        public async Task<User?> ValidateUserAsync(LoginDTO logindDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=> u.Email == logindDto.Email);

            if(user == null || !BCrypt.Net.BCrypt.Verify(logindDto.Password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("UserName", user.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes( _config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    issuer: _config["JwtSettings:Issuer"],
                    audience: _config["JwtSettings:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(15),
                    signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}
