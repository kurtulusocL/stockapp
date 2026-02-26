
namespace StockManagement.Shared.Dtos.MappingDtos.WarehouseDtos
{
    public class WarehouseGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public string TypeOfWarehouse { get; set; }
        public int ProductCount { get; set; }
        public int UnitInStockCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
