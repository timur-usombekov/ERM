
namespace ERM.Application.DTOs
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public Guid? SeamstressId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public bool IsSeamstress { get; set; }
        public string? MachineNumber { get; set; }
    }
}
