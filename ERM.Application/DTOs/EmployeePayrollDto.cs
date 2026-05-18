namespace ERM.Application.DTOs
{
    public class EmployeePayrollDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string MachineNumber { get; set; } = string.Empty;

        public bool IsSeamstress { get; set; }
        public bool IsCutter { get; set; }

        public int TotalItemsProcessed { get; set; }
        public decimal EarnedByOperations { get; set; } 
        public decimal TotalAdjustments { get; set; }
        public decimal TotalToPay => EarnedByOperations + TotalAdjustments;

        public List<PayrollDetailDto> OperationDetails { get; set; } = []; 
        public List<AdjustmentDetailDto> AdjustmentDetails { get; set; } = [];
    }

}