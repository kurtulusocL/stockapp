
namespace StockManagement.Shared.Dtos.MappingDtos.ProductDtos
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string AppUserId { get; set; }
        public int WarehouseId { get; set; }
    }
}
