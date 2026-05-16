using ERM.Core.Domain.Entities.Base;

namespace ERM.Core.Domain.Entities
{
    public class CutBatch : Identity
    {
        public string Title { get; private set; } = null!;
        public DateOnly Date { get; private set; }

        public bool IsClosed { get; private set; }

        public int DeclaredQuantity { get; private set; }

        public IReadOnlyCollection<CutBatchItem> Items => _items.AsReadOnly();
        private readonly List<CutBatchItem> _items = [];

        protected CutBatch() { }

        public CutBatch(string title, DateOnly date, int declaredQuantity)
        {
            if (declaredQuantity <= 0)
                throw new ArgumentException("Количество от закройщика должно быть больше нуля.");


            Title = title;
            Date = date;
            DeclaredQuantity = declaredQuantity;
            IsClosed = false;
        }
        /// <summary>
        /// </summary>
        /// <returns> true, если был добавлен новый элемент, false если количество было увеличено для существующего элемента </returns>
        public bool AddItem(Guid clothingModelId, Guid fabricColorId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Количество позиции должно быть больше нуля.");

            int currentlyDistributed = _items.Sum(i => i.Quantity);

            if (currentlyDistributed + quantity > DeclaredQuantity)
                throw new InvalidOperationException(
                    $"Невозможно добавить {quantity} шт. Превышен лимит от закройщика! " +
                    $"Осталось: {DeclaredQuantity - currentlyDistributed} шт.");

            // что бы избежать дубликатов, количество может быть увеличено для уже существующей позиции 
            // с тем же сочетанием модели одежды и цвета ткани, для того же кроя
            var existingItem = _items.FirstOrDefault(i =>
                i.ClothingModelId == clothingModelId &&
                i.FabricColorId == fabricColorId);

            if (existingItem != null)
            {
                existingItem.IncreaseQuantity(quantity);
                return false;
            }
            else
            {
                _items.Add(new CutBatchItem(Id, clothingModelId, fabricColorId, quantity));
                return true;
            }
        }

        public void Close() => IsClosed = true;
        public void Open() => IsClosed = false;
    }

}