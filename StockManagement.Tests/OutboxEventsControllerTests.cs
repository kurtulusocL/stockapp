using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockManagement.API.Controllers;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.MappingDtos.OutboxEventDtos;

namespace StockManagement.Tests
{
    public class OutboxEventsControllerTests
    {
        private readonly Mock<IOutboxEventService> _mockOutboxEventService;
        private readonly OutboxEventsController _controller;

        public OutboxEventsControllerTests()
        {
            _mockOutboxEventService = new Mock<IOutboxEventService>();
            _controller = new OutboxEventsController(_mockOutboxEventService.Object);
        }

        [Fact]
        public async Task GetAllOutboxEvents_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<OutboxEventGetDto>
        {
            new() { Id = 1, EntityType = "Product", EventType = "Added" },
            new() { Id = 2, EntityType = "Category", EventType = "Modified" }
        };
            _mockOutboxEventService.Setup(s => s.GetAllAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllOutboxEvents();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllOutboxEvents_ReturnsOk_WithEmptyList()
        {
            // Arrange
            _mockOutboxEventService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<OutboxEventGetDto>());

            // Act
            var result = await _controller.GetAllOutboxEvents();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<OutboxEventGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllOutboxEventsBySuccessfullProcess_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<OutboxEventGetDto>
        {
            new() { Id = 1, EntityType = "Product", EventType = "Added" }
        };
            _mockOutboxEventService.Setup(s => s.GetAllBySuccessfullProcessAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllOutboxEventsBySuccessfullProcess();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllOutboxEventsBySuccessfullProcess_ReturnsOk_WithEmptyList_WhenNoneSuccessful()
        {
            // Arrange
            _mockOutboxEventService.Setup(s => s.GetAllBySuccessfullProcessAsync()).ReturnsAsync(new List<OutboxEventGetDto>());

            // Act
            var result = await _controller.GetAllOutboxEventsBySuccessfullProcess();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<OutboxEventGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllOutboxEventsByErrorProcess_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<OutboxEventGetDto>
        {
            new() { Id = 2, EntityType = "Warehouse", EventType = "Deleted" }
        };
            _mockOutboxEventService.Setup(s => s.GetAllByErrorProcessAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllOutboxEventsByErrorProcess();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetAllOutboxEventsByErrorProcess_ReturnsOk_WithEmptyList_WhenNoErrors()
        {
            // Arrange
            _mockOutboxEventService.Setup(s => s.GetAllByErrorProcessAsync()).ReturnsAsync(new List<OutboxEventGetDto>());

            // Act
            var result = await _controller.GetAllOutboxEventsByErrorProcess();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            (ok!.Value as List<OutboxEventGetDto>).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllWholeOutboxEvents_ReturnsOk_WithList()
        {
            // Arrange
            var fakeData = new List<OutboxEventGetDto>
        {
            new() { Id = 1, EntityType = "Product", EventType = "Added" }
        };
            _mockOutboxEventService.Setup(s => s.GetAllAllDataAsync()).ReturnsAsync(fakeData);

            // Act
            var result = await _controller.GetAllWholeOutboxEvents();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task GetOutboxEventById_ReturnsOk_WhenFound()
        {
            // Arrange
            var fakeDto = new OutboxEventGetDto { Id = 1, EntityType = "Product", EventType = "Added" };
            _mockOutboxEventService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(fakeDto);

            // Act
            var result = await _controller.GetOutboxEventById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public async Task GetOutboxEventById_ReturnsNotFound_WhenNull()
        {
            // Arrange
            _mockOutboxEventService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((OutboxEventGetDto?)null);

            // Act
            var result = await _controller.GetOutboxEventById(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockOutboxEventService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "It couldn't be deleted.." };
            _mockOutboxEventService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.Delete(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteOutboxEventById_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockOutboxEventService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.DeleteOutboxEventById(ids);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteOutboxEventById_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var ids = new List<int> { 99, 100 };
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Bulk deletion failed." };
            _mockOutboxEventService.Setup(s => s.DeleteByIdAsync(ids)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.DeleteOutboxEventById(ids);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockOutboxEventService.Setup(s => s.SetActiveAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetActive(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Activation failed." };
            _mockOutboxEventService.Setup(s => s.SetActiveAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetActive(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetInActive_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockOutboxEventService.Setup(s => s.SetInActiveAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetInActive(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetInActive_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "It couldn't be made passive." };
            _mockOutboxEventService.Setup(s => s.SetInActiveAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetInActive(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetDeleted_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockOutboxEventService.Setup(s => s.SetDeletedAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetDeleted(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetDeleted_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Soft delete failed." };
            _mockOutboxEventService.Setup(s => s.SetDeletedAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetDeleted(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetNotDeleted_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var successResult = new ServiceResult<bool> { IsSuccess = true, Data = true };
            _mockOutboxEventService.Setup(s => s.SetNotDeletedAsync(1)).ReturnsAsync(successResult);

            // Act
            var result = await _controller.SetNotDeleted(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetNotDeleted_ReturnsBadRequest_WhenFailed()
        {
            // Arrange
            var failResult = new ServiceResult<bool> { IsSuccess = false, Data = false, Message = "Restore failed." };
            _mockOutboxEventService.Setup(s => s.SetNotDeletedAsync(99)).ReturnsAsync(failResult);

            // Act
            var result = await _controller.SetNotDeleted(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
