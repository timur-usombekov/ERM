namespace ERM.Application.DTOs
{
    public class CutBatchDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public int DeclaredQuantity { get; set; }
        public int UnallocatedQuantity { get; set; } // Для UI
        public bool IsClosed { get; set; }

        public List<CutBatchItemDto> Items { get; set; } = [];
    }
}