namespace ERM.Application.DTOs
{
    public class WorkAssignmentDto
    {
        public Guid Id { get; set; }

        // Инфо о швее
        public string SeamstressName { get; set; } = string.Empty;
        public string MachineNumber { get; set; } = string.Empty;

        // Инфо о крое
        public string ClothingModelName { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Quantity { get; set; }

        public DateOnly AssignedDate { get; set; }
    }
}