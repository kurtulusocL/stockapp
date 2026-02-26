
namespace StockManagement.Shared.Dtos.MappingDtos.ProductDtos
{
    public class ProductGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string AppUserId { get; set; }
        public int WarehouseId { get; set; }
        public int StockMovementCount { get; set; }
        public int UnitInStockCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
