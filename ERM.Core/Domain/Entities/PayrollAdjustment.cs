using ERM.Core.Domain.Entities.Base;

namespace ERM.Core.Domain.Entities
{
    public class PayrollAdjustment : Identity
    {
        public Guid SeamstressId { get; private set; }
        public DateOnly Date { get; private set; }

        // Положительное число = доп плата, Отрицательное = штраф
        public decimal Amount { get; private set; }
        public string Reason { get; private set; } = null!;

        public Seamstress Seamstress { get; private set; } = null!;

        protected PayrollAdjustment() { }

        public PayrollAdjustment(Guid seamstressId, DateOnly date, decimal amount, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Причина корректировки не может быть пустой.");
            if (amount == 0)
                throw new ArgumentException("Сумма корректировки не может быть равна нулю.");

            SeamstressId = seamstressId;
            Date = date;
            Amount = amount;
            Reason = reason;
        }
    }
}