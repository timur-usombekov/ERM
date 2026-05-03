using ERM.Core.Domain.Entities.Base;
using System.Globalization;

namespace ERM.Core.Domain.Entities
{
    public class WorkAssignment : Identity
    {
        public Guid SeamstressId { get; private set; }
        public Guid ClothingModelId { get; private set; }

        public string Size { get; private set; } = null!;      // "52", "XL"
        public string? Color { get; private set; }
        public int AssignedQuantity { get; private set; }      // выдано
        public int CompletedQuantity { get; private set; }     // сделано

        public DateOnly AssignedDate { get; private set; }
        public int WeekNumber { get; private set; }            // для подсчёта зарплаты
        public int Year { get; private set; }

        public Seamstress Seamstress { get; private set; } = null!;
        public ClothingModel ClothingModel { get; private set; } = null!;

        protected WorkAssignment() { }

        public WorkAssignment(
            Guid seamstressId,
            Guid clothingModelId,
            string size,
            string? color,
            int assignedQuantity)
        {
            SeamstressId = seamstressId;
            ClothingModelId = clothingModelId;
            Size = size;
            Color = color;
            AssignedQuantity = assignedQuantity;
            CompletedQuantity = 0; // ещё ничего не сделано
            AssignedDate = DateOnly.FromDateTime(DateTime.Today);

            // ISO 8601 — неделя считается по стандарту
            var dateTime = AssignedDate.ToDateTime(TimeOnly.MinValue);
            WeekNumber = ISOWeek.GetWeekOfYear(dateTime);
            Year = ISOWeek.GetYear(dateTime);
        }

        public void Complete(int completedQuantity)
        {
            if (completedQuantity < 0 || completedQuantity > AssignedQuantity)
                throw new InvalidOperationException(
                    $"Некорректное количество: {completedQuantity}. Ожидается от 0 до {AssignedQuantity}.");

            CompletedQuantity = completedQuantity;
        }
    }
}