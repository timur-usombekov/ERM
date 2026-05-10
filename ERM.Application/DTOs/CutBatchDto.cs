namespace ERM.Application.DTOs
{
    public class CutBatchDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public int DeclaredQuantity { get; set; }
        public int UnallocatedQuantity { get; set; } // Для UI

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