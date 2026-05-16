public class CutBatchItemDto
{
    public Guid Id { get; set; }
    public Guid ClothingModelId { get; set; }
    public string ClothingModelName { get; set; } = string.Empty; // Для UI
    public string Color { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int AvailableQuantity { get; set; }
}