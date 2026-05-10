using ERM.Core.Domain.Entities.Base;
using System.Globalization;

namespace ERM.Core.Domain.Entities
{
    public class WorkAssignment : Identity
    {
        public Guid SeamstressId { get; private set; }

        public Guid CutBatchItemId { get; private set; }

        public string Size { get; private set; } = null!; // "52", "XL"
        public int Quantity { get; private set; }

        public DateOnly AssignedDate { get; private set; }
        public int WeekNumber { get; private set; }
        public int Year { get; private set; }

        public Seamstress Seamstress { get; private set; } = null!;
        public CutBatchItem CutBatchItem { get; private set; } = null!;

        protected WorkAssignment() { }

        public WorkAssignment(
            Guid seamstressId,
            Guid cutBatchItemId,
            string size,
            int quantity)
        {
            SeamstressId = seamstressId;
            CutBatchItemId = cutBatchItemId;
            Size = size;
            Quantity = quantity;

            AssignedDate = DateOnly.FromDateTime(DateTime.Today);

            var dateTime = AssignedDate.ToDateTime(TimeOnly.MinValue);
            WeekNumber = ISOWeek.GetWeekOfYear(dateTime);
            Year = ISOWeek.GetYear(dateTime);
        }
    }
}