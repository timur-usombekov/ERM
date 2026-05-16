namespace ERM.Application.DTOs
{
    public class ClothingModelDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal SewingPrice { get; set; }
        public string? Description { get; set; }
    }
}
