using StockManagement.Domain.EntityBase.EntityFramework;

namespace StockManagement.Domain.Entities
{
    public class Warehouse : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public string TypeOfWarehouse { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<UnitInStock> UnitInStocks { get; set; }
    }
}
