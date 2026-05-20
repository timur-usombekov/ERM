namespace ERM.Application.DTOs
{
    public class PayrollDetailDto
    {
        public string OperationName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public decimal PricePerUnit { get; set; }
        public int Quantity { get; set; }
        public decimal Sum => PricePerUnit * Quantity;
    }
}
