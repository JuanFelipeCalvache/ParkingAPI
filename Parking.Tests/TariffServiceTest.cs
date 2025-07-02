using Xunit;
using Parking.Services;
using Parking.Models;
using Parking.Repositories.Interfaces;
using Moq;
using Parking.DTOs;


namespace Parking.Parking.Tests
{
    public class TariffServiceTest
    {

        private readonly Mock<ITariffRepository> _mockTariffRepo = new();

        private TariffService _service;

        public TariffServiceTest()
        {
            _service = new TariffService(_mockTariffRepo.Object);
        }


        [Fact]
        public async Task GetAllAsyn_ReturnsTariffDTOList_WhenTariffsExist()
        {
            //Arrange
            var tariffs = new List<Tariff>
            {
                new Tariff { Id = 1, RatePerHour = 10, VehicleType = "Carro"},
                new Tariff {Id = 2, RatePerHour = 5, VehicleType = "Moto"}
            };

            _mockTariffRepo.Setup(t => t.GetAllTariffs()).ReturnsAsync(tariffs);

            //Act
            var result = await _service.GetAllAsync();


            //Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Carro", result[0].VehicleType);
            Assert.Equal("Moto", result[1].VehicleType);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoTariffExists()
        {
            //Arrange
            _mockTariffRepo.Setup(t => t.GetAllTariffs()).ReturnsAsync(new List<Tariff>());

            //Act
            var result = await _service.GetAllAsync();


            //Assert
            Assert.NotNull(result);
            Assert.Empty(result);


        }

        [Fact]
        public async Task GetTariffAsync_ReturnsTariffDTO_WhenTariffExists() 
        {
            //Arrange
            var tariff = new Tariff { Id = 1, RatePerHour = 10, VehicleType = "Carro"};

            _mockTariffRepo.Setup(t => t.GetTariffByVehicle(tariff.VehicleType)).ReturnsAsync(tariff);

            //Act
            var result = await _service.GetTariffAsync(tariff.VehicleType);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(tariff.VehicleType, result.VehicleType);
            Assert.Equal(10, result.RatePerHour);
        }

        [Fact]
        public async Task GetTariffAsync_ReturnsNull_WhenTariffDoesNotExist() 
        {
            //Arrange
            var vehicleType = "Moto";

            _mockTariffRepo.Setup(t => t.GetTariffByVehicle(vehicleType)).ReturnsAsync((Tariff)null);

            //Act
            var result = await _service.GetTariffAsync(vehicleType);

            //Assert
            Assert.Null(result);

        }


        [Fact]
        public async Task AddTariffAsync_CallsAddMethodInRepository()
        {
            // Arrange
            var tariffDTO = new TariffDTO
            {
                VehicleType = "Carro",
                RatePerHour = 2000
            };

            // Act
            await _service.AddTariffAsync(tariffDTO);

            // Assert
            _mockTariffRepo.Verify(repo => repo.AddTariffAsync(It.Is<Tariff>(t =>
                t.VehicleType == tariffDTO.VehicleType &&
                t.RatePerHour == tariffDTO.RatePerHour)), Times.Once);
        }

        [Fact]
        public async Task UpdateTariffAsync_ReturnsTrue_WhenTariffExists()
        {
            //Arrange
            var tariffDTO = new TariffDTO
            {
                id = 1,
                VehicleType = "Carro",
                RatePerHour = 2000

            };

            var existingTariff = new Tariff
            {
                Id = 1,
                VehicleType = "Bicleta",
                RatePerHour = 1000
            };

            _mockTariffRepo.Setup(r => r.GetTariffById(tariffDTO.id)).ReturnsAsync(existingTariff);
            _mockTariffRepo.Setup(r => r.UpdateTariffAsync(It.IsAny<Tariff>())).ReturnsAsync(true);


            //Act
            var result = await _service.UpdateTariffAsync(tariffDTO);

            //Assert
            Assert.True(result);
            _mockTariffRepo.Verify(r => r.UpdateTariffAsync(It.Is<Tariff>(
                    t => t.VehicleType == "Carro" && t.RatePerHour == 2000
                )), Times.Once);

        }

        [Fact]
        public async Task UpdateTriffAsync_ReturnsFalse_WhenTariffDoesNotExists()
        {
            //Arrange
            var tariffDTO = new TariffDTO
            {
                id = 99,
                VehicleType = "Carro",
                RatePerHour = 2000
            };

            _mockTariffRepo.Setup(t => t.GetTariffById(99)).ReturnsAsync((Tariff)null);

            //Act
            var result = await _service.UpdateTariffAsync(tariffDTO);

            //Assert
            Assert.False(result);
            _mockTariffRepo.Verify(t => t.UpdateTariffAsync(It.IsAny<Tariff>()), Times.Never);

        }
    }
}
