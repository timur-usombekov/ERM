using ERM.Core.Domain.Entities.Base;

namespace ERM.Core.Domain.Entities
{
    public class Cutter : Identity
    {
        public Guid EmployeeId { get; private set; }

        public decimal Percentage { get; private set; }

        public Employee Employee { get; private set; } = null!;

        protected Cutter() { }

        public Cutter(Guid employeeId, decimal percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("Процент закройщика должен быть от 0 до 100.");

            EmployeeId = employeeId;
            Percentage = percentage;
        }

        public void UpdatePercentage(decimal percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("Процент закройщика должен быть от 0 до 100.");
            Percentage = percentage;
        }
    }
}