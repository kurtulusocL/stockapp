using System.Text.Json.Serialization;
using StockManagement.Domain.EntityBase.EntityFramework;

namespace StockManagement.Domain.Entities
{
    public class Audit : BaseEntity
    {
        public string AreaAccessed { get; set; }

        public string? AppUserId { get; set; }

        [JsonIgnore]
        public virtual AppUser AppUser { get; set; }
    }
}
