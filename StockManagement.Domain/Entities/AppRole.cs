using Microsoft.AspNetCore.Identity;
using StockManagement.Domain.EntityBase;

namespace StockManagement.Domain.Entities
{
    public class AppRole : IdentityRole, IEntity
    {
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
