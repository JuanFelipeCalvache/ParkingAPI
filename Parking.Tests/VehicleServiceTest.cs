using Xunit;
using Parking.Services;
using Parking.Models;
using Parking.DTOs;
using Parking.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;


namespace Parking.Parking.Tests
{
    public class VehicleServiceTest
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;

            return new AppDbContext(options);
        }

        private IConfiguration GetFakeConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"SomeKey", "SomeValue" }
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public async Task GetAllVehiclesAsync_ReturnsVehicles()
        {
            //arrange
            var context = GetInMemoryDbContext();
            context.Vehicles.Add(new Vehicle { Id = 1, NumberPlate = "ABC123", Type = "Car", Owner = "Juan" });
            context.Vehicles.Add(new Vehicle { Id = 2, NumberPlate = "JDK12", Type = "Moto", Owner = "Ana" });
            await context.SaveChangesAsync();

            var service = new VehicleService(context, GetFakeConfiguration());

            //Act
            var result = await service.GetAllVehiclesAsync();

            //Assert 
            Assert.Equal(2, result.Count);
            Assert.Contains(result, v => v.Plate == "ABC123");
        }


        [Fact]
        public async Task DeleteVehicleAsync_RemovesVehicle_WhenExists()
        {
            //arrange 
            var context = GetInMemoryDbContext();
            context.Vehicles.Add(new Vehicle { Id = 1, NumberPlate = "DELETE1", Type = "Car", Owner = "Luis" });
            await context.SaveChangesAsync();

            var service = new VehicleService(context, GetFakeConfiguration());

            //Act
            var result = await service.DeleteVehicleAsync(1);

            //Assert
            Assert.True(result);
            Assert.Empty(context.Vehicles);
        }


        [Fact]
        public async Task DeleteVehicleAsync_ReturnsFalse_WhenNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new VehicleService(context, GetFakeConfiguration());

            // Act
            var result = await service.DeleteVehicleAsync(999);

            // Assert
            Assert.False(result);
        }


    }
}

