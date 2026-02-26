using StockManagement.Domain.EntityBase.EntityFramework;

namespace StockManagement.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
