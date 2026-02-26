using System.Text.Json.Serialization;
using StockManagement.Domain.EntityBase.EntityFramework;

namespace StockManagement.Domain.Entities
{
    public class UserSession:BaseEntity
    {
        public string Username { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public bool IsOnline { get; set; }
        public int? OnlineDurationSeconds { get; set; }

        public string AppUserId { get; set; }

        [JsonIgnore]
        public virtual AppUser AppUser { get; set; }
    }
}
