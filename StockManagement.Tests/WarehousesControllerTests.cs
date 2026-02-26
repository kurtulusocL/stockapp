using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockManagement.API.Controllers;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.MappingDtos.WarehouseDtos;

namespace StockManagement.Tests
{
    public class WarehousesControllerTests
    {
        private readonly Mock<IWarehouseService> _mockWarehouseService;
        private readonly WarehousesController _controller;

        public WarehousesControllerTests()
        {
            _mockWarehouseService = new Mock<IWarehouseService>();
            _controller = new WarehousesController(_mockWarehouseService.Object);
        }

        [Fact]
        public async Task GetAllWarehouses_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<WarehouseGetDto>
        {
            new() { Id = 1, Name = "Warehouse A", Code = "DA01" },
            new() { Id = 2, Name = "Warehouse B", Code = "DB01" }
        };
            _mockWarehouseService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllWarehouses();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllWarehouses_ReturnsOk_WithEmptyList()
        {
            // Arrange
            _mockWarehouseService.Setup(s => s.GetAllIncludingAsync()).ReturnsAsync(new List<WarehouseGetDto>());

            // Act
            var result = await _controller.GetAllWarehouses();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<WarehouseGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllWholeWarehouses_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<WarehouseGetDto>
        {
            new() { Id = 1, Name = "Warehouse A", Code = "DA01" }
        };
            _mockWarehouseService.Setup(s => s.GetAllIncludingByAllDataAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllWholeWarehouses();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetWarehouseById_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeDto = new WarehouseGetDto { Id = 1, Name = "Warehouse A", Code = "DA01" };
            _mockWarehouseService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(fakeDto);

            // Act
            var result = await _controller.GetWarehouseById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public async Task GetWarehouseById_ReturnsNotFound_WhenNull()
        {
            // Arrange
            _mockWarehouseService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((WarehouseGetDto?)null);

            // Act
            var result = await _controller.GetWarehouseById(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetForEdit_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeDto = new WarehouseUpdateDto { Id = 1, Name = "Warehouse A", Code = "DA01" };
            _mockWarehouseService.Setup(s => s.GetForEditAsync(1)).ReturnsAsync(fakeDto);

            // Act
            var result = await _controller.GetForEdit(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public async Task GetForEdit_ReturnsNotFound_WhenNull()
        {
            // Arrange
            _mockWarehouseService.Setup(s => s.GetForEditAsync(99)).ReturnsAsync((WarehouseUpdateDto?)null);

            // Act
            var result = await _controller.GetForEdit(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task WarehouseCreate_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var dto = new WarehouseCreateDto { Name = "Warehouse A", Code = "DA01", Address = "İstanbul", TypeOfWarehouse = "General" };
            var successResult = new ServiceResult<bool> { IsSuccess = true };
            _mockWarehouseService
                .Setup(s => s.CreateAsync(dto.Name, dto.Code, dto.Address, dto.TypeOfWarehouse))
                .ReturnsAsync(successResult);

            // Act
            var result = await _controller.WarehouseCreate(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task WarehouseCreate_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var dto = new WarehouseCreateDto { Name = "Warehouse A", Code = "DA01", Address = "İstanbul", TypeOfWarehouse = "General" };
            var failResult = new ServiceResult<bool> { IsSuccess = false, Message = "Oluşturulamadı." };
            _mockWarehouseService
                .Setup(s => s.CreateAsync(dto.Name, dto.Code, dto.Address, dto.TypeOfWarehouse))
                .ReturnsAsync(failResult);

            // Act
            var result = await _controller.WarehouseCreate(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var bad = result as BadRequestObjectResult;
            bad!.Value.Should().Be("Oluşturulamadı.");
        }

        [Fact]
        public async Task WarehouseUpdate_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var dto = new WarehouseUpdateDto { Id = 1, Name = "Warehouse A", Code = "DA01", Address = "Ankara", TypeOfWarehouse = "Cold" };
            var successResult = new ServiceResult<bool> { IsSuccess = true };
            _mockWarehouseService
                .Setup(s => s.UpdateAsync(dto.Name, dto.Code, dto.Address, dto.TypeOfWarehouse, dto.Id))
                .ReturnsAsync(successResult);

            // Act
            var result = await _controller.WarehouseUpdate(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task WarehouseUpdate_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var dto = new WarehouseUpdateDto { Id = 1, Name = "Warehouse A", Code = "DA01", Address = "Ankara", TypeOfWarehouse = "Cold" };
            var failResult = new ServiceResult<bool> { IsSuccess = false, Message = "Could not be updated.." };
            _mockWarehouseService
                .Setup(s => s.UpdateAsync(dto.Name, dto.Code, dto.Address, dto.TypeOfWarehouse, dto.Id))
                .ReturnsAsync(failResult);

            // Act
            var result = await _controller.WarehouseUpdate(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var bad = result as BadRequestObjectResult;
            bad!.Value.Should().Be("Could not be updated..");
        }

        [Fact]
        public async Task DeleteWarehouseById_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true };
            _mockWarehouseService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.DeleteWarehouseById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteWarehouseById_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Message = "It couldn't be deleted." };
            _mockWarehouseService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.DeleteWarehouseById(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };
            var successResult = new ServiceResult<bool> { IsSuccess = true };
            _mockWarehouseService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.DeleteMultiple(ids);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var ids = new List<int> { 99, 100 };
            var failResult = new ServiceResult<bool> { IsSuccess = false, Message = "Bulk deletion failed." };
            _mockWarehouseService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.DeleteMultiple(ids);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true };
            _mockWarehouseService.Setup(s => s.SetActiveAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetActive(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Message = "Activation failed." };
            _mockWarehouseService.Setup(s => s.SetActiveAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetActive(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetInactive_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true };
            _mockWarehouseService.Setup(s => s.SetInActiveAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetInactive(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetInactive_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Message = "It couldn't be made passive." };
            _mockWarehouseService.Setup(s => s.SetInActiveAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetInactive(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetDeleted_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true };
            _mockWarehouseService.Setup(s => s.SetDeletedAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetDeleted(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetDeleted_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Message = "Soft delete failed." };
            _mockWarehouseService.Setup(s => s.SetDeletedAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetDeleted(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetNotDeleted_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true };
            _mockWarehouseService.Setup(s => s.SetNotDeletedAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetNotDeleted(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetNotDeleted_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Message = "Restore failed." };
            _mockWarehouseService.Setup(s => s.SetNotDeletedAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetNotDeleted(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
