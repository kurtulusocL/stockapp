
namespace StockManagement.Shared.Dtos.MappingDtos.StockMovementDtos
{
    public class StockMovementGetDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string MovementType { get; set; }
        public string AppUserId { get; set; }
        public int? ProductId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
