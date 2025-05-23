using Microsoft.EntityFrameworkCore;
using Parking.DTOs;
using Parking.Data;
using Parking.Models;
using Parking.interfaces;

namespace Parking.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly  AppDbContext _context;
        private readonly IConfiguration _config;


        public VehicleService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;  

        }


        public async Task<List<VehicleDTO>> GetAllVehiclesAsync()
        {
            var vehicles = await _context.Vehicles
                .Select(v => new VehicleDTO
                {
                    Id = v.Id,
                    Plate = v.NumberPlate,
                    Type = v.Type,
                    Owner = v.Owner
                })
                .ToListAsync();
            return vehicles;
        }


        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null) 
            {
                return false;
            }

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();

            return true;



        }

    }
}
