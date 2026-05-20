
namespace ERM.Application.DTOs
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Notes { get; set; }

        // Швея
        public bool IsSeamstress { get; set; }
        public Guid? SeamstressId { get; set; }
        public string? MachineNumber { get; set; }

        // Закройщик
        public bool IsCutter { get; set; }
        public decimal? CutterPercentage { get; set; }

        public bool IsIroner { get; set; }

        public bool HasNoRole => !IsSeamstress && !IsCutter && !IsIroner;


        //  отображение для UI
        public string DisplayInfo =>
            (IsSeamstress ? $"[Швея №{MachineNumber}] " : "") +
            (IsCutter ? $"[Закройщик] " : "") +
            (IsIroner ? $"[Гладильщица] " : "") +
            FullName;

    }

}
