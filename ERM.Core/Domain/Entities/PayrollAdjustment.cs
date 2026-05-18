using ERM.Core.Domain.Entities.Base;

namespace ERM.Core.Domain.Entities
{
    public class PayrollAdjustment : Identity
    {
        public Guid EmployeeId { get; private set; }
        public Employee Employee { get; private set; } = null!;

        public DateOnly Date { get; private set; }

        // Положительное число = доп плата, Отрицательное = штраф
        public decimal Amount { get; private set; }
        public string Reason { get; private set; } = null!;

        protected PayrollAdjustment() { }

        public PayrollAdjustment(Guid employeeId, DateOnly date, decimal amount, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Укажите причину.");
            if (amount == 0) throw new ArgumentException("Сумма не может быть нулем.");

            EmployeeId = employeeId;
            Date = date;
            Amount = amount;
            Reason = reason;
        }

    }
}