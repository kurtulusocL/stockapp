
namespace StockManagement.Shared.Dtos.MappingDtos.CategoryDtos
{
    public class CategoryUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
