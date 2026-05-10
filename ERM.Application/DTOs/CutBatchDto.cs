namespace ERM.Application.DTOs
{
    public class CutBatchDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly Date { get; set; }

        public List<CutBatchItemDto> Items { get; set; } = [];
    }

    public class CutBatchItemDto
    {
        public Guid Id { get; set; }
        public Guid ClothingModelId { get; set; }
        public string ClothingModelName { get; set; } = string.Empty; // Для UI
        public string Color { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}