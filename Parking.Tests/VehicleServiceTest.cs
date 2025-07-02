using Xunit;
using Parking.Services;
using Parking.Models;
using Parking.Repositories.Interfaces;
using Moq;


namespace Parking.Parking.Tests
{
    public class VehicleServiceTest
    {
        private readonly Mock<IVehicleRepository> _mockVehicleRepo = new();

        private VehicleService _service;

        public VehicleServiceTest()
        {
            _service = new VehicleService(_mockVehicleRepo.Object);
        }


        [Fact]

        public async Task GetAllVehicles_ReturnsMappedVehicleDTOs()
        {
            //Arrange
            var vehicles = new List<Vehicle>
            {
                new Vehicle { Id = 1, NumberPlate = "ABC123", Type = "Carro", Owner = "Jose"},
                new Vehicle { Id = 2, NumberPlate = "JDC123", Type = "Carro", Owner = "Maria"},
            };

            _mockVehicleRepo.Setup(v => v.GetAllVehiclesAsync()).ReturnsAsync(vehicles);

            //Act
            var result = await _service.GetAllVehiclesAsync();

            //Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("ABC123", result[0].Plate);
            Assert.Equal("JDC123", result[1].Plate);

        }


        [Fact]

        public async Task GetAllVehiclesAsync_NoVehicles_ReturnsEmptyList()
        {
            _mockVehicleRepo.Setup(v => v.GetAllVehiclesAsync()).ReturnsAsync(new List<Vehicle>());

            //Act
            var result = await _service.GetAllVehiclesAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            
            

        }

        [Fact]
        public async Task DeleteVehicleAsync_VehicleExists_ReturnTrue()
        {
            //Arrange
            var vehicle = new Vehicle { Id = 1, NumberPlate = "ABC123", Type = "Car", Owner = "Maria" };

            _mockVehicleRepo.Setup(v => v.GetByIdAsync(1)).ReturnsAsync(vehicle);
            _mockVehicleRepo.Setup(v => v.DeleteAsync(vehicle)).Returns(Task.CompletedTask);

            //Act
            var result = await _service.DeleteVehicleAsync(1);


            //Assert
            Assert.True(result);

        }

        [Fact]
        public async Task DeleteVehicle_VehicleNotFound_ReturnsFalse()
        {
            //Arrange
            _mockVehicleRepo.Setup(v => v.GetByIdAsync(1)).ReturnsAsync((Vehicle)null);

            //Act
            var result = await _service.DeleteVehicleAsync(1);

            //Assert
            Assert.False(result);


        }






    }
}


