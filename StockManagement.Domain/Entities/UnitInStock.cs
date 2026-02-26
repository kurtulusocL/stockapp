using StockManagement.Domain.EntityBase.EntityFramework;

namespace StockManagement.Domain.Entities
{
    public class UnitInStock : BaseEntity
    {
        public int Quantity { get; set; }
        public string Code { get; set; }

        public int? ProductId { get; set; }
        public int WarehouseId { get; set; }
        public string AppUserId { get; set; }

        public virtual Product Product { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual AppUser AppUser { get; set; }
    }
}
