using ERM.Core.Domain.Entities.Base;

namespace ERM.Core.Domain.Entities
{
    public class CutBatch : Identity
    {
        public string Title { get; private set; } = null!;
        public DateOnly Date { get; private set; }

        public IReadOnlyCollection<CutBatchItem> Items => _items.AsReadOnly();
        private readonly List<CutBatchItem> _items = [];

        protected CutBatch() { }

        public CutBatch(string title, DateOnly date)
        {
            Title = title;
            Date = date;
        }

        public void AddItem(Guid clothingModelId, string color, int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Количество должно быть больше нуля.");

            _items.Add(new CutBatchItem(Id, clothingModelId, color, quantity));
        }
    }
}