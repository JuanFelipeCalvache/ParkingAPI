using Moq;
using Xunit;
using Parking.Services;
using Parking.Repositories;
using Parking.Repositories.Interfaces;
using Parking.Services.interfaces;
using Parking.Models;
using Parking.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Parking.Parking.Tests
{
    public class EntryExitServiceTest
    {
        private readonly Mock<IEntryExitRepository> _mockEntryExitRepo = new();
        private readonly Mock<ITariffService> _mockTariffService = new();
        private readonly Mock<IVehicleRepository> _mockVehicleRepo = new();
        private readonly Mock<ISpaceRepository> _mockSpaceRepo = new();
        private readonly Mock<ITariffRepository> _mockTariffRepo = new();

        private EntryExitService _service;

        public EntryExitServiceTest()
        {
            _service = new EntryExitService(
                _mockEntryExitRepo.Object,
                _mockTariffService.Object,
                _mockVehicleRepo.Object,
                _mockSpaceRepo.Object,
                _mockTariffRepo.Object
                );
        }

        [Fact]
        public async Task RegisterEntryAsync_VehicleAlreadyInside_ReturnsError()
        {
            //Arrange
            var entryDTO = new EntryDTO { SpaceId = 1 };
            var vehicleDTO = new VehicleDTO { Plate = "ABC123", Type = "Carro", Owner = "Jose" };

            _mockVehicleRepo.Setup(v => v.GetVehicleByPlate("ABC123")).ReturnsAsync(new Vehicle { Id = 1, NumberPlate = "ABC123" });
            _mockEntryExitRepo.Setup(r => r.GetActiveEntryByPlateAsync("ABC123")).ReturnsAsync(new EntryExit { Id = 1 });

            //Act
            var result = await _service.RegisterEntryAsync(entryDTO, vehicleDTO);

            //Assert
            Assert.False(result.Success);
            Assert.Equal("The vehicle is already inside the parking lot", result.Message);

        }
        [Fact]
        public async Task RegisterEntryAsync_NewVehicle_AdEntrySuccessfully()
        {
            // Arrange
            var entryDTO = new EntryDTO { SpaceId = 1 };
            var vehicleDTO = new VehicleDTO { Plate = " xyz789 ", Type = "Carro", Owner = "Pipe" };
            var normalizedPlate = vehicleDTO.Plate.Trim().ToUpper();

            _mockEntryExitRepo.Setup(r => r.GetActiveEntryByPlateAsync(normalizedPlate)).ReturnsAsync((EntryExit)null);
            _mockVehicleRepo.Setup(v => v.GetVehicleByPlate(normalizedPlate)).ReturnsAsync((Vehicle)null);
            _mockSpaceRepo.Setup(s => s.GetAvailableSpaceByIdAsync(1)).ReturnsAsync(new Space { Id = 1, IsOccupied = false });
            _mockTariffRepo.Setup(t => t.GetTariffByVehicle("Carro")).ReturnsAsync(new Tariff { Id = 1, RatePerHour = 2000, VehicleType = "Carro" });
            _mockTariffService.Setup(t => t.CalculateFee(It.IsAny<EntryExit>(), It.IsAny<decimal>())).Returns(2000);

            _mockVehicleRepo.Setup(v => v.AddVehicleAsync(It.IsAny<Vehicle>())).Returns(Task.CompletedTask);
            _mockEntryExitRepo.Setup(e => e.AddAsync(It.IsAny<EntryExit>())).Returns(Task.CompletedTask);
            _mockSpaceRepo.Setup(s => s.UpdateSpaceAsync(It.IsAny<Space>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.RegisterEntryAsync(entryDTO, vehicleDTO);

            // Assert
            Assert.True(result.Success, $"Expected success, but got: {result.Message}");
            Assert.Equal("XYZ789", result.VehiclePlate);
        }

        [Fact]
        public async Task GetAllEntriesExits_ReturnsMappedEntriExitDTO()
        {
            //Arrange
            var entryTime = DateTime.Now.AddHours(-2);
            var vehicle = new Vehicle { Id = 1, NumberPlate = "ABC123", Type = "Carro", Owner = "Jose" };
            var space = new Space { Id = 1 };

            var entries = new List<EntryExit>
            {
                new EntryExit { Id = 1,EntryTime = entryTime, ExitTime = null, Vehicle = vehicle, Space = space }
            };

            var tariff = new Tariff { Id = 1, RatePerHour = 10, VehicleType = "carro" };


            _mockEntryExitRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(entries);
            _mockTariffRepo.Setup(r => r.GetAllTariffs()).ReturnsAsync(new List<Tariff> { tariff });
            _mockTariffService.Setup(s => s.CalculateFee(It.IsAny<EntryExit>(), 10)).Returns(20);

            //Act
            var result = await _service.GetAllEntriesExitsAsync();

            //Assert
            Assert.Single(result);
            var dto = result[0];
            Assert.True(dto.Success);
            Assert.Equal("ABC123", dto.VehiclePlate);
            Assert.Equal("1", dto.SpaceCode);
            Assert.Equal(20, dto.AmountToPay);
            Assert.Equal(entryTime, dto.EntryTime);
            Assert.Null(dto.ExitTime);

        }

        [Fact]
        public async Task RegisterEntryAsync_SpaceUnavailable_ReturnsError()
        {
            var entryDTO = new EntryDTO { SpaceId = 2};
            var vehicleDTO = new VehicleDTO { Plate = "ABC123", Type = "Moto", Owner = " Jese" };

            _mockVehicleRepo.Setup(r => r.GetVehicleByPlate("ABC123")).ReturnsAsync((Vehicle)null);
            _mockEntryExitRepo.Setup(r => r.GetActiveEntryByPlateAsync("ABC123")).ReturnsAsync((EntryExit)null);
            _mockSpaceRepo.Setup(s => s.GetSpaceById(2)).ReturnsAsync((Space)null);

            //Act
            var result = await _service.RegisterEntryAsync(entryDTO, vehicleDTO);

            //Assert
            Assert.False(result.Success);
            Assert.Equal("Space is not available.", result.Message);

        }

        [Fact]
        public async Task GetAllEntriesExitsAsync_NoTariff_ReturnsError()
        {
            //Arrange
            _mockEntryExitRepo.Setup(e => e.GetAllAsync()).ReturnsAsync(new List<EntryExit>());
            _mockTariffRepo.Setup(r => r.GetAllTariffs()).ReturnsAsync((List<Tariff>)null);

            //Act
            var result = await _service.GetAllEntriesExitsAsync();

            Assert.Single(result);
            Assert.False(result[0].Success);
            Assert.Equal("Tariff configuration not found", result[0].Message);

        }

        [Fact]
        public async Task DeleteEntryExitAsync_RecordNotFound_ReturnsFalse()
        {
            //Arrange
            _mockEntryExitRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((EntryExit)null);

            //Act
            var result = await _service.DeleteEntryExitAsync(1);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RegisterExitByPlateAsync_ShouldReturnSuccess_WhenExitIsRegisteredCorrectly()
        {
            var plate = "ABC123";
            var exitTime = DateTime.Now;
            var rate = 5m;

            var vehicle = new Vehicle { Id = 1, NumberPlate = plate };
            var space = new Space { Id = 1, IsOccupied = true, VehicleId = 1 };
            var activeEntry = new EntryExit
            {
                Id = 1,
                Vehicle = vehicle,
                VehicleId = 1,
                SpaceId = 1,
                EntryTime = DateTime.Now.AddHours(-2), // 2 horas dentro
                ExitTime = null,
                FeeToPaid = null
            };

            var exitDTO = new ExitDTO
            {
                ExitTime = exitTime,
                TariffDTO = new TariffDTO { RatePerHour = rate }
            };


            _mockEntryExitRepo.Setup(repo => repo.GetActiveEntryByPlateAsync(plate))
                .ReturnsAsync(activeEntry);
            _mockSpaceRepo.Setup(repo => repo.GetSpaceById(1))
                .ReturnsAsync(space);
            _mockTariffService.Setup(service => service.CalculateFee(It.IsAny<EntryExit>(), rate))
                .Returns(10m); // por ejemplo, 2 horas x $5/h


            // Act
            var result = await _service.RegisterExitByPlateAsync(plate, exitDTO);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(1, result.Id);
            Assert.Equal(plate, result.VehiclePlate);
            Assert.Equal("1", result.SpaceCode);
            Assert.Equal(10m, result.AmountToPay);
            Assert.Equal(exitTime, result.ExitTime);

            // Verifica que se hayan llamado los métodos esperados
            _mockSpaceRepo.Verify(r => r.UpdateSpaceAsync(It.Is<Space>(s => s.IsOccupied == false && s.VehicleId == null)), Times.Once);
            _mockEntryExitRepo.Verify(r => r.UpdateAsync(It.Is<EntryExit>(e => e.FeeToPaid == 10m && e.ExitTime == exitTime)), Times.Once);
        }

        [Fact]
        public async Task RegisterExitByPlate_ActiveEntryNotFound_ReturnsError()
        {
            //Arrange
            var plate = "ABC123";
            var exitDTO = new ExitDTO
            {
                ExitTime = DateTime.Now,
                TariffDTO = new TariffDTO { RatePerHour = 10}
            };

            _mockEntryExitRepo.Setup(r => r.GetActiveEntryByPlateAsync(plate)).ReturnsAsync((EntryExit)null);

            //Act
            var result = await _service.RegisterExitByPlateAsync(plate, exitDTO);

            //Assert
            Assert.False(result.Success);
            Assert.Equal("Active entry not found for this vehicle.", result.Message);

        }

        [Fact]
        public async Task RegisterExitByPlate_InvalidTariff_ReturnsError()
        {
            //Arrange
            var vehicle = new Vehicle { Id = 1, NumberPlate = "ADG12C", Type = "Moto", Owner = "Jose" };
            var space = new Space { Id = 1, IsOccupied = true, VehicleId = 1 };

            var activeEntry = new EntryExit
            {
                Id = 1,
                Vehicle = vehicle,
                VehicleId = 1,
                SpaceId = 1,
                EntryTime = DateTime.Now.AddHours(-2)
            };

            var exitDTO = new ExitDTO
            {
                ExitTime = DateTime.Now,
                TariffDTO = new TariffDTO { RatePerHour = 0 }
            };

            _mockEntryExitRepo.Setup(r => r.GetActiveEntryByPlateAsync(vehicle.NumberPlate)).ReturnsAsync(activeEntry);

            //Act

            var result = await _service.RegisterExitByPlateAsync(vehicle.NumberPlate, exitDTO);

            //Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid or missing tariff information.", result.Message);


        }

        [Fact]
        public async Task RegisterExitByPlate_NoTariff_ReturnsErrorMessage()
        {
            //Arrange
            _mockEntryExitRepo.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(new List<EntryExit>());
            _mockTariffRepo.Setup(t => t.GetAllTariffs()).ReturnsAsync(new List<Tariff>());

            //Act
            var result = await _service.GetEntrysInParking();

            //Assert
            Assert.Single(result);
            Assert.False(result[0].Success);
            Assert.Equal("Tariff configuration not found", result[0].Message);
        }

        [Fact]
        public async Task RegisterEntryAsync_ExistingVehicle_AddEntryCorrectly()
        {
            var entryDTO = new EntryDTO { SpaceId = 1 };
            var vehicleDTO = new VehicleDTO { Plate = "JDK123", Type = "Carro", Owner = "Maria" };
            var normalizedPlate = vehicleDTO.Plate.Trim().ToUpper();
            var vehicle = new Vehicle { Id = 2, NumberPlate = normalizedPlate, Type = "Carro", Owner = "Maria" };

            _mockVehicleRepo.Setup(v => v.GetVehicleByPlate(normalizedPlate)).ReturnsAsync(vehicle);
            _mockEntryExitRepo.Setup(r => r.GetActiveEntryByPlateAsync(normalizedPlate)).ReturnsAsync((EntryExit?)null);
            _mockSpaceRepo.Setup(s => s.GetAvailableSpaceByIdAsync(entryDTO.SpaceId)).ReturnsAsync(new Space { Id = 1, IsOccupied = false });
            _mockEntryExitRepo.Setup(e => e.AddAsync(It.IsAny<EntryExit>())).Returns(Task.CompletedTask);
            _mockSpaceRepo.Setup(s => s.UpdateSpaceAsync(It.IsAny<Space>())).Returns(Task.CompletedTask);

            //Act
            var result = await _service.RegisterEntryAsync(entryDTO, vehicleDTO);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(normalizedPlate, result.VehiclePlate);

        }

        [Fact]
        public async Task DeleteEntryExitAsync_RecordExists_ReturnsTrue()
        {
            var entry = new EntryExit { Id = 1 };

            _mockEntryExitRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entry);
            _mockEntryExitRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _service.DeleteEntryExitAsync(1);

            Assert.True(result);
        }




    }
}

