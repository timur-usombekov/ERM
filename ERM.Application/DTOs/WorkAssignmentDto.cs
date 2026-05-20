namespace ERM.Application.DTOs
{
    public class WorkAssignmentDto
    {
        public Guid Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string MachineNumber { get; set; } = string.Empty;
        public string ClothingModelName { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateOnly AssignedDate { get; set; }

        public string OperationName { get; set; } = string.Empty;
    }

}