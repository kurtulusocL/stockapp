
namespace StockManagement.Shared.Dtos.MappingDtos.UnitInStockStos
{
    public class UnitInStockCreateDto
    {
        public int Quantity { get; set; }
        public string Code { get; set; }
        public int? ProductId { get; set; }
        public int WarehouseId { get; set; }
        public string AppUserId { get; set; }
    }
}
