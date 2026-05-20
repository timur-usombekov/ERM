using ERM.Core.Domain.Entities.Base;
using ERM.Core.Domain.Entities.Enum;
using System.Globalization;

namespace ERM.Core.Domain.Entities
{
    public class WorkAssignment : Identity
    {
        public Guid EmployeeId { get; private set; }
        public Employee Employee { get; private set; } = null!;

        public OperationType OperationType { get; private set; }

        public Guid? CutBatchItemId { get; private set; }
        public CutBatchItem? CutBatchItem { get; private set; }

        public string? Size { get; private set; } = null!; // "52", "XL"

        public int Quantity { get; private set; }
        public decimal PricePerUnit { get; private set; }
        public decimal TotalPrice => Quantity * PricePerUnit; // Это не хранится в базе, а вычисляется при запросе


        public DateOnly AssignedDate { get; private set; }
        public int WeekNumber { get; private set; }
        public int Year { get; private set; }


        protected WorkAssignment() { }

        public WorkAssignment(
            Guid employeeId,
            OperationType operationType,
            int quantity,
            decimal pricePerUnit,
            Guid? cutBatchItemId = null,
            string? size = null)
        {
            if (quantity == 0) throw new ArgumentException("Количество не может быть равно нулю.");
            if (operationType == OperationType.Sewing && cutBatchItemId == null)
                throw new ArgumentException("Пошив требует привязки к партии кроя.");

            EmployeeId = employeeId;
            OperationType = operationType;
            Quantity = quantity;
            PricePerUnit = pricePerUnit;

            CutBatchItemId = cutBatchItemId;
            Size = size;

            AssignedDate = DateOnly.FromDateTime(DateTime.Today);
            var dateTime = AssignedDate.ToDateTime(TimeOnly.MinValue);
            WeekNumber = ISOWeek.GetWeekOfYear(dateTime);
            Year = ISOWeek.GetYear(dateTime);
        }

    }
}