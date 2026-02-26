using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using StockManagement.Domain.EntityBase;

namespace StockManagement.Domain.Entities
{
    public class AppUser : IdentityUser, IEntity
    {
        public string NameSurname { get; set; }
        public string Title { get; set; }
        public string PhoneNumber { get; set; }
        public int? ConfirmCode { get; set; }
        public bool IsLoginConfirmCodeActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        [JsonIgnore]
        public virtual ICollection<Audit> Audits { get; set; }

        [JsonIgnore]
        public virtual ICollection<StockMovement> StockMovements { get; set; }

        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }

        [JsonIgnore]
        public virtual ICollection<UnitInStock> UnitInStocks { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserSession> UserSessions { get; set; }
        public AppUser()
        {
            PhoneNumberConfirmed = true;
            EmailConfirmed = true;
        }
    }
}
