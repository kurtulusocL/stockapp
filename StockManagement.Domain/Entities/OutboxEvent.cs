using StockManagement.Domain.EntityBase.EntityFramework;

namespace StockManagement.Domain.Entities
{
    public class OutboxEvent : BaseEntity
    {
        public string EntityType { get; set; }
        public string EventType { get; set; }
        public string Payload { get; set; }
        public bool IsProcessed { get; set; } = false;
        public DateTime ProcessedDate { get; set; }
    }
}
