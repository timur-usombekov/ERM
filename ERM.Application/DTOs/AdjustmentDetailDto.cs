namespace ERM.Application.DTOs
{
    public class AdjustmentDetailDto
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

}
