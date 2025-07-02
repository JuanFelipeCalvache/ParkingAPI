using Xunit;
using Parking.Services;
using Parking.Models;
using Parking.Repositories.Interfaces;
using Moq;
using Parking.DTOs;

namespace Parking.Parking.Tests
{
    public class SpacesServiceTest
    {
        private readonly Mock<ISpaceRepository> _mockSpaceRepo = new();

        private SpaceService _service;

        public SpacesServiceTest()
        {
            _service = new SpaceService(_mockSpaceRepo.Object);
        }

        [Fact]
        public async Task GetllSpacesAsync_ReturnMappedSpacesDTO()
        {
            //Arrange
            _mockSpaceRepo.Setup(r => r.GetAllSpacesAsync()).ReturnsAsync(new List<Space>
            {
                new Space { Id = 1, IsOccupied = false, VehicleId = null },
                new Space { Id = 12, IsOccupied = true, VehicleId = 100 },
            });

            //Act
            var result = await _service.GetAllSpacesAsync();


            //Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Id);
            Assert.False(result[0].IsOccupied);
            Assert.Null(result[0].VehicleId);
        }

        [Fact]
        public async Task AddSpace_CallRepositoryWithCorrectData()
        {
            //Arrange
            var spaceDTO = new SpaceDTO { IsOccupied = true, VehicleId = 100 };

            //Act
            await _service.AddSpace(spaceDTO);

            //Assert
            _mockSpaceRepo.Verify(s => s.AddSpaceAsync(It.Is<Space>(
                s => s.IsOccupied == spaceDTO.IsOccupied && s.VehicleId == spaceDTO.VehicleId
                )), Times.Once);

        }


        [Fact]
        public async Task ChangeStateSpace_ExistingSpace_UpdateAndReturnsTrue()
        {
            //Arrange
            var space = new Space { Id = 1, IsOccupied = false, VehicleId = null };

            _mockSpaceRepo.Setup(s => s.GetSpaceById(1)).ReturnsAsync(space);

            var spaceDTO = new SpaceDTO { Id = 1, IsOccupied = true, VehicleId = 75 };

            //Act
            var result = await _service.ChangeStateSpace(spaceDTO);

            //Assert
            Assert.True(result);
            _mockSpaceRepo.Verify(s => s.UpdateSpaceAsync(It.Is<Space>(
                s => s.IsOccupied == true && s.VehicleId == 75)), Times.Once);



        }


        [Fact]
        public async Task ChangeStateSpace_NonExistingSpace_ReturnsFalse()
        {
            //Arrange
            _mockSpaceRepo.Setup(r => r.GetSpaceById(It.IsAny<int>())).ReturnsAsync((Space?)null);

            var dto = new SpaceDTO { Id = 99, IsOccupied = true, VehicleId = 10 };

            var result = await _service.ChangeStateSpace(dto);

            Assert.False(result);
            _mockSpaceRepo.Verify(r => r.UpdateSpaceAsync(It.IsAny<Space>()), Times.Never);
        }




    }
}
