
namespace StockManagement.Shared.Dtos.MappingDtos.UnitInStockStos
{
    public class UnitInStockUpdateDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string Code { get; set; }
        public int? ProductId { get; set; }
        public int WarehouseId { get; set; }
        public string AppUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
