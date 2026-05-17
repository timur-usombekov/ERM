namespace ERM.Application.DTOs
{
    public class SeamstressPayrollDto
    {
        public Guid SeamstressId { get; set; }
        public string SeamstressName { get; set; } = string.Empty;
        public string MachineNumber { get; set; } = string.Empty;

        public int TotalItemsSewn { get; set; }

        public decimal EarnedBySewing { get; set; } // Заработано сдельно (за пошив)
        public decimal TotalAdjustments { get; set; } // Всякие приколы типа надбавок и штрафов (может быть отрицательным)
        public decimal TotalToPay => EarnedBySewing + TotalAdjustments; 

        public List<PayrollDetailDto> SewingDetails { get; set; } = []; // За пошив
        public List<AdjustmentDetailDto> AdjustmentDetails { get; set; } = []; // Авансы/Штрафы
    }

}