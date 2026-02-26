
namespace StockManagement.Shared.Dtos.MappingDtos.UnitInStockStos
{
    public class UnitInStockGetDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string Code { get; set; }
        public int? ProductId { get; set; }
        public int WarehouseId { get; set; }
        public string AppUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
