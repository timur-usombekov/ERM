using ERM.Core.Domain.Entities.Base;

namespace ERM.Core.Domain.Entities
{
    public class CutBatchItem : Identity
    {
        public Guid CutBatchId { get; private set; }
        public Guid ClothingModelId { get; private set; }

        public string Color { get; private set; } = null!;
        public int Quantity { get; private set; }

        public CutBatch CutBatch { get; private set; } = null!;
        public ClothingModel ClothingModel { get; private set; } = null!;

        protected CutBatchItem() { }

        public CutBatchItem(Guid cutBatchId, Guid clothingModelId, string color, int quantity)
        {
            CutBatchId = cutBatchId;
            ClothingModelId = clothingModelId;
            Color = color;
            Quantity = quantity;
        }
    }
}