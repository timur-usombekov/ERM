using ERM.Core.Domain.Entities.Base;

namespace ERM.Core.Domain.Entities
{
    public class CutBatchItem : Identity
    {
        public Guid CutBatchId { get; private set; }
        public Guid ClothingModelId { get; private set; }
        public Guid FabricColorId { get; private set; }

        public int Quantity { get; private set; }
        public int IssuedQuantity { get; private set; }

        public decimal CutPricePerUnit { get; private set; }

        public CutBatch CutBatch { get; private set; } = null!;
        public ClothingModel ClothingModel { get; private set; } = null!;
        public FabricColor FabricColor { get; private set; } = null!;

        protected CutBatchItem() { }

        public CutBatchItem(Guid cutBatchId, Guid clothingModelId, Guid fabricColorId, int quantity, decimal cutPricePerUnit)
        {
            CutBatchId = cutBatchId;
            ClothingModelId = clothingModelId;
            FabricColorId = fabricColorId;
            Quantity = quantity;
            CutPricePerUnit = cutPricePerUnit;
            IssuedQuantity = 0;
        }

        public void Issue(int quantityToIssue)
        {
            if (quantityToIssue <= 0)
                throw new ArgumentException("Количество выдачи должно быть больше нуля.");

            if (IssuedQuantity + quantityToIssue > Quantity)
                throw new InvalidOperationException($"Невозможно выдать {quantityToIssue} шт. Доступный остаток: {Quantity - IssuedQuantity} шт.");

            IssuedQuantity += quantityToIssue;
        }

        public void CancelIssue(int quantityToReturn)
        {
            if (IssuedQuantity - quantityToReturn < 0)
                throw new InvalidOperationException("Нельзя вернуть больше, чем было выдано.");

            IssuedQuantity -= quantityToReturn;
        }

        public void IncreaseQuantity(int amount)
        {
            if (amount <= 0) throw new ArgumentException("Количество должно быть больше нуля.");
            Quantity += amount;
        }

        public void UpdateDetails(Guid clothingModelId, Guid fabricColorId, int newQuantity, decimal newCutPricePerUnit)
        {
            // Защита: нельзя поставить количество меньше, чем уже успели выдать швеям
            if (newQuantity < IssuedQuantity)
                throw new InvalidOperationException(
                    $"Нельзя установить количество {newQuantity}. Уже выдано в работу {IssuedQuantity} шт.");

            ClothingModelId = clothingModelId;
            FabricColorId = fabricColorId;
            Quantity = newQuantity;
            CutPricePerUnit = newCutPricePerUnit;
        }


    }
}