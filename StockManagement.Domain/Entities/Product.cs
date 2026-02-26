using StockManagement.Domain.EntityBase.EntityFramework;

namespace StockManagement.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }
        public string AppUserId { get; set; }
        public int WarehouseId { get; set; }

        public virtual Category Category { get; set; }
        public virtual AppUser AppUser { get; set; }
        public virtual Warehouse Warehouse { get; set; }

        public virtual ICollection<StockMovement> StockMovements { get; set; }
        public virtual  ICollection<UnitInStock> UnitInStocks { get; set; }
    }
}
