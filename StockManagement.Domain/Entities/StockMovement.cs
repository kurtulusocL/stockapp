using StockManagement.Domain.EntityBase.EntityFramework;

namespace StockManagement.Domain.Entities
{
    public class StockMovement : BaseEntity
    {        
        public int Quantity { get; set; }
        public string MovementType { get; set; }

        public string AppUserId { get; set; }
        public int? ProductId { get; set; }

        public virtual AppUser AppUser { get; set; }
        public virtual Product Product { get; set; }
    }
}
