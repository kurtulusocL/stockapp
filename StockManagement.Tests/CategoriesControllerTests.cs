using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockManagement.API.Controllers;
using StockManagement.Business.Constants.Utilities.Results;
using StockManagement.Business.Services.Abstract;
using StockManagement.Shared.Dtos.MappingDtos.CategoryDtos;

namespace StockManagement.Tests
{
    public class CategoriesControllerTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _controller = new CategoriesController(_mockCategoryService.Object);
        }

        [Fact]
        public async Task GetAllCategories_ReturnsOk_WithData()
        {
            // Arrange
            var categories = new List<CategoryGetDto>
            {
                new CategoryGetDto { Id = 1, Name = "Test Category" },
                new CategoryGetDto { Id = 2, Name = "Test Category 2" }
            };
            _mockCategoryService.Setup(x => x.GetAllIncludingAsync()).ReturnsAsync(categories);

            // Act
            var result = await _controller.GetAllCategories();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var data = okResult.Value.Should().BeAssignableTo<IEnumerable<CategoryGetDto>>().Subject;
            data.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsOk_WhenExists()
        {
            // Arrange
            var category = new CategoryGetDto { Id = 1, Name = "Test" };
            _mockCategoryService.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(category);

            // Act
            var result = await _controller.GetCategoryById(1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(category);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            _mockCategoryService.Setup(x => x.GetByIdAsync(99))
                .ReturnsAsync((CategoryGetDto)null);

            // Act
            var result = await _controller.GetCategoryById(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CategoryCreate_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var dto = new CategoryCreateDto { Name = "New Category", Code = "NC001" };
            var serviceResult = new ServiceResult<bool> { IsSuccess = true };
            _mockCategoryService.Setup(x => x.CrateAsync(dto))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.CategoryCreate(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CategoryCreate_ReturnsBadRequest_WhenFails()
        {
            // Arrange
            var dto = new CategoryCreateDto { Name = "New Category", Code = "NC001" };
            var serviceResult = new ServiceResult<bool> { IsSuccess = false, Message = "Error" };
            _mockCategoryService.Setup(x => x.CrateAsync(dto))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.CategoryCreate(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetAllWholeCategories_ReturnsOk_WithData()
        {
            // Arrange
            var categories = new List<CategoryGetDto>
    {
        new CategoryGetDto { Id = 1, Name = "Test Category" },
        new CategoryGetDto { Id = 2, Name = "Test Category 2" }
    };
            _mockCategoryService.Setup(x => x.GetAllIncludingAllDataAsync()).ReturnsAsync(categories);

            // Act
            var result = await _controller.GetAllWholeCategories();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var data = okResult.Value.Should().BeAssignableTo<IEnumerable<CategoryGetDto>>().Subject;
            data.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetForEdit_ReturnsOk_WhenExists()
        {
            // Arrange
            var dto = new CategoryUpdateDto { Id = 1, Name = "Test" };
            _mockCategoryService.Setup(x => x.GetForEditAsync(1)).ReturnsAsync(dto);

            // Act
            var result = await _controller.GetForEdit(1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(dto);
        }

        [Fact]
        public async Task GetForEdit_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            _mockCategoryService.Setup(x => x.GetForEditAsync(99)).ReturnsAsync((CategoryUpdateDto)null);

            // Act
            var result = await _controller.GetForEdit(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CategoryUpdate_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var dto = new CategoryUpdateDto { Id = 1, Name = "Updated Category" };
            var serviceResult = new ServiceResult<bool> { IsSuccess = true };
            _mockCategoryService.Setup(x => x.UpdateAsync(dto)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.CategoryUpdate(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CategoryUpdate_ReturnsBadRequest_WhenFails()
        {
            // Arrange
            var dto = new CategoryUpdateDto { Id = 1, Name = "Updated Category" };
            var serviceResult = new ServiceResult<bool> { IsSuccess = false, Message = "Error" };
            _mockCategoryService.Setup(x => x.UpdateAsync(dto)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.CategoryUpdate(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteCategory_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var serviceResult = new ServiceResult<bool> { IsSuccess = true };
            _mockCategoryService.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.DeleteCategory(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteCategory_ReturnsBadRequest_WhenFails()
        {
            // Arrange
            var serviceResult = new ServiceResult<bool> { IsSuccess = false, Message = "Error" };
            _mockCategoryService.Setup(x => x.DeleteAsync(99)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.DeleteCategory(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };
            var serviceResult = new ServiceResult<bool> { IsSuccess = true };
            _mockCategoryService.Setup(x => x.DeleteByIdAsync(ids)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.DeleteMultiple(ids);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_ReturnsBadRequest_WhenFails()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };
            var serviceResult = new ServiceResult<bool> { IsSuccess = false, Message = "Error" };
            _mockCategoryService.Setup(x => x.DeleteByIdAsync(ids)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.DeleteMultiple(ids);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var serviceResult = new ServiceResult<bool> { IsSuccess = true };
            _mockCategoryService.Setup(x => x.SetActiveAsync(1)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.SetActive(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsBadRequest_WhenFails()
        {
            // Arrange
            var serviceResult = new ServiceResult<bool> { IsSuccess = false, Message = "Error" };
            _mockCategoryService.Setup(x => x.SetActiveAsync(99)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.SetActive(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetInactive_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var serviceResult = new ServiceResult<bool> { IsSuccess = true };
            _mockCategoryService.Setup(x => x.SetInActiveAsync(1)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.SetInactive(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetInactive_ReturnsBadRequest_WhenFails()
        {
            // Arrange
            var serviceResult = new ServiceResult<bool> { IsSuccess = false, Message = "Error" };
            _mockCategoryService.Setup(x => x.SetInActiveAsync(99)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.SetInactive(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetDeleted_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var serviceResult = new ServiceResult<bool> { IsSuccess = true };
            _mockCategoryService.Setup(x => x.SetDeletedAsync(1)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.SetDeleted(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetDeleted_ReturnsBadRequest_WhenFails()
        {
            // Arrange
            var serviceResult = new ServiceResult<bool> { IsSuccess = false, Message = "Error" };
            _mockCategoryService.Setup(x => x.SetDeletedAsync(99)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.SetDeleted(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetNotDeleted_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var serviceResult = new ServiceResult<bool> { IsSuccess = true };
            _mockCategoryService.Setup(x => x.SetNotDeletedAsync(1)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.SetNotDeleted(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetNotDeleted_ReturnsBadRequest_WhenFails()
        {
            // Arrange
            var serviceResult = new ServiceResult<bool> { IsSuccess = false, Message = "Error" };
            _mockCategoryService.Setup(x => x.SetNotDeletedAsync(99)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.SetNotDeleted(99);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
