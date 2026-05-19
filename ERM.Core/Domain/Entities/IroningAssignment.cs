using ERM.Core.Domain.Entities.Base;
using System.Globalization;

namespace ERM.Core.Domain.Entities
{
    public class IroningAssignment : Identity
    {
        public Guid IronerId { get; private set; }             // чья смена
        public Guid? SubstituteSeamstressId { get; private set; } // кто фактически работал (null = сама гладильщица)
        public Guid CutBatchItemId { get; private set; }

        public int Quantity { get; private set; }
        public decimal PricePerUnit { get; private set; }      // snapshot на момент записи

        public decimal TotalPrice => Quantity * PricePerUnit;

        public DateOnly AssignedDate { get; private set; }
        public int WeekNumber { get; private set; }
        public int Year { get; private set; }

        // Навигация
        public Ironer Ironer { get; private set; } = null!;
        public Seamstress? SubstituteSeamstress { get; private set; }
        public CutBatchItem CutBatchItem { get; private set; } = null!;

        public bool IsSubstitution => SubstituteSeamstressId.HasValue;

        protected IroningAssignment() { }

        public IroningAssignment(
            Guid ironerId,
            Guid cutBatchItemId,
            int quantity,
            decimal pricePerUnit,
            Guid? substituteSeamstressId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Количество должно быть больше нуля.");
            if (pricePerUnit < 0)
                throw new ArgumentException("Цена не может быть отрицательной.");

            IronerId = ironerId;
            CutBatchItemId = cutBatchItemId;
            SubstituteSeamstressId = substituteSeamstressId;
            Quantity = quantity;
            PricePerUnit = pricePerUnit;

            AssignedDate = DateOnly.FromDateTime(DateTime.Today);
            var dt = AssignedDate.ToDateTime(TimeOnly.MinValue);
            WeekNumber = ISOWeek.GetWeekOfYear(dt);
            Year = ISOWeek.GetYear(dt);
        }
    }
}
