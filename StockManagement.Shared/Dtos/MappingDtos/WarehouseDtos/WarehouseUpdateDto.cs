
namespace StockManagement.Shared.Dtos.MappingDtos.WarehouseDtos
{
    public class WarehouseUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public string TypeOfWarehouse { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
