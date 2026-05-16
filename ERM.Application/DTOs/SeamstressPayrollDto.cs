namespace ERM.Application.DTOs
{
    public class SeamstressPayrollDto
    {
        public Guid SeamstressId { get; set; }
        public string SeamstressName { get; set; } = string.Empty;
        public string MachineNumber { get; set; } = string.Empty;

        public int TotalItemsSewn { get; set; } 
        public decimal TotalSalary { get; set; }

        public List<PayrollDetailDto> Details { get; set; } = [];
    }

}