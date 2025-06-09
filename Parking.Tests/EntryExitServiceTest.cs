using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Parking.Services;
using Parking.Data;
using Parking.DTOs;
using Parking.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Parking.Services.interfaces;

namespace Parking.Parking.Tests
{
    public class EntryExitServiceTest
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private IConfiguration GetFakeConfiguration()
        {
            var settings = new Dictionary<string, string> { { "SomeKey", "SomeValue" } };
            return new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        }

        [Fact]
        public async Task RegisterEntryAsync_Should_Add_Entry_When_Vehicle_Not_Exists_And_Space_Is_Free()
        {
            //arrange
            var dbContext = GetInMemoryDbContext();

            var mockTariffService = new Mock<ITariffService>();
            var config = GetFakeConfiguration();

            //agregar espacio disponible
            var space = new Space { Id = 1, IsOccupied = false };
            dbContext.Add(space);
            await dbContext.SaveChangesAsync();

            var service = new EntryExitService(dbContext, config, mockTariffService.Object);

            var vehicleDTO = new VehicleDTO
            {
                Plate = "ABC123",
                Type = "Carro",
                Owner = "Juan Pérez"
            };

            var entryDTO = new EntryDTO
            {
                SpaceId = 1
            };

            //Act
            var result = await service.RegisterEntryAsync(entryDTO, vehicleDTO);

            //Assert
            Assert.True(result.Success);
            Assert.Equal("ABC123", result.VehiclePlate);
            Assert.Equal("1", result.SpaceCode);

            //Confirmar que el espacio quedo ocupado
            var updateSpcae = await dbContext.Spaces.FirstAsync(s => s.Id == 1);
            Assert.True(updateSpcae.IsOccupied);


        }

        [Fact]
        public async Task RegisterExitByPlateAsync_ShouldRegisterExitAndCalculateFee()
        {
            // Arrange
            var plate = "XYZ789";
            var vehicle = new Vehicle { NumberPlate = plate, Type = "Car", Owner = "Alice" };
            var space = new Space { Id = 1, IsOccupied = true, VehicleId = 1 };

            var exitTime = DateTime.UtcNow;
            var entryTime = DateTime.UtcNow.AddHours(-2);

            var context = GetInMemoryDbContext();
            context.Vehicles.Add(vehicle);
            context.Spaces.Add(space);

            var entry = new EntryExit
            {
                Id = 1,
                EntryTime = entryTime,
                VehicleId = vehicle.Id,
                Vehicle = vehicle,
                SpaceId = space.Id,
                Space = space
            };

            context.EntryExits.Add(entry);
            await context.SaveChangesAsync();

            var tariffServiceMock = new Mock<ITariffService>();
            tariffServiceMock
                .Setup(x => x.GetTariffAsync(It.IsAny<string>()))
                .ReturnsAsync(new TariffDTO { RatePerHour = 5, VehicleType = "Carro" });

            tariffServiceMock
                .Setup(x => x.CalculateFee(It.IsAny<EntryExit>(), It.IsAny<decimal>()))
                .Returns<EntryExit, decimal>((e, rate) =>
                {
                    var hours = (decimal)Math.Ceiling((e.ExitTime.Value - e.EntryTime).TotalHours);
                    return hours * rate;
                });

            var configMock = new Mock<IConfiguration>();
            var service = new EntryExitService(context, configMock.Object, tariffServiceMock.Object);

            var exitDto = new ExitDTO
            {
                ExitTime = exitTime,
                TariffDTO = new TariffDTO { VehicleType = "Carro", RatePerHour = 5 }
            };

            // Act
            var result = await service.RegisterExitByPlateAsync(plate, exitDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(10, result.AmountToPay); 
            Assert.Equal(exitTime, result.ExitTime);
            Assert.Equal("XYZ789", vehicle.NumberPlate);
            Assert.False(context.Spaces.First().IsOccupied);
        }

        [Fact]
        public async Task GetAllEntriesExitsAsync_ShouldReturn_ValidResponse()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockTariffService = new Mock<ITariffService>();
            var config = GetFakeConfiguration();

            // Agregar Tarifa
            var tariff = new Tariff { Id = 1, RatePerHour = 5, VehicleType = "Carro" };

            await context.Tariffs.AddAsync(tariff);

            // Agregar Vehículo y Espacio
            var vehicle = new Vehicle { Id = 1, Type = "Carro", NumberPlate = "ABC123", Owner = "Jose" };
            var space = new Space { Id = 1, IsOccupied = true };

            await context.Vehicles.AddAsync(vehicle);
            await context.Spaces.AddAsync(space);
            await context.SaveChangesAsync(); // ¡IMPORTANTE!

            // Agregar Entrada/Salida
            var entryExit = new EntryExit
            {
                Id = 1,
                EntryTime = DateTime.UtcNow.AddHours(-2),
                ExitTime = DateTime.UtcNow,
                VehicleId = vehicle.Id,
                SpaceId = space.Id,
                FeeToPaid = 10
            };
            await context.EntryExits.AddAsync(entryExit);
            await context.SaveChangesAsync();

            // Servicio
            var service = new EntryExitService(context, config, mockTariffService.Object);

            // Act
            var result = await service.GetAllEntriesExitsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.True(result[0].Success);
            Assert.Equal("ABC123", result[0].VehiclePlate);
            Assert.Equal("1", result[0].SpaceCode);
            Assert.Equal(10, result[0].AmountToPay);
        }


        [Fact]
        public async Task GetEntrysInParking_ReturnsOnlyActiveEntries()
        {
            //arrange
            var context = GetInMemoryDbContext();
            var mockTariffSerice = new Mock<ITariffService>();
            var config = GetFakeConfiguration();

            var tariff = new Tariff { Id = 1, RatePerHour = 5, VehicleType = "Carro" };
            await context.Tariffs.AddAsync(tariff);

            var vehicleOne = new Vehicle { Id = 1, NumberPlate = "ABC123", Type = "Carro", Owner = "Juan" };
            var vehicleTwo = new Vehicle { Id = 2, NumberPlate = "AYZ123", Type = "Carro", Owner = "JOSE" };

            var space = new Space { Id = 1, IsOccupied = true, VehicleId = 1 };
            var spaceTwo = new Space { Id = 2, IsOccupied = true, VehicleId = 2 };

            await context.Vehicles.AddAsync(vehicleOne);
            await context.Vehicles.AddAsync(vehicleTwo);
            await context.Spaces.AddAsync(space);
            await context.Spaces.AddAsync(spaceTwo);

            await context.SaveChangesAsync();

            var activeEntry = new EntryExit 
            { 
                Id = 1, 
                EntryTime = DateTime.UtcNow.AddHours(-1), 
                ExitTime = null, 
                VehicleId = 1, 
                SpaceId = 1, 
                FeeToPaid = 0
            };

            var ExitEntry = new EntryExit
            {
                Id = 2,
                EntryTime = DateTime.UtcNow.AddHours(-3),
                ExitTime = DateTime.UtcNow.AddHours(-2),
                VehicleId = 2,
                SpaceId = 2,
                FeeToPaid = 0
            };

            await context.EntryExits.AddAsync(activeEntry);
            await context.EntryExits.AddAsync(ExitEntry);

            await context.SaveChangesAsync();


            //Servicio
            var service = new EntryExitService(context, config, mockTariffSerice.Object);

            //act
            var result = await service.GetEntrysInParking();

            //Assert
            var entry = result.First();
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Null(result.First().ExitTime);
            Assert.Equal("ABC123", result.First().VehiclePlate);
            Assert.Equal("1", entry.SpaceCode);
            Assert.InRange(entry.EntryTime, DateTime.UtcNow.AddHours(-1.1), DateTime.UtcNow.AddMinutes(-59));


        }




    }
}

