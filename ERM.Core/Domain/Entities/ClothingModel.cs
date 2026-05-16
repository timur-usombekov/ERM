using ERM.Core.Domain.Entities.Base;

namespace ERM.Core.Domain.Entities
{
    public class ClothingModel : Identity
    {
        public string Name { get; private set; } = null!;
        public decimal SewingPrice { get; private set; }
        public string? Description { get; private set; }

        protected ClothingModel() { }

        public ClothingModel(string name, decimal sewingPrice, string? description = null)
        {
            if (sewingPrice < 0) throw new ArgumentException("Цена пошива не может быть отрицательной.");
            Name = name;
            SewingPrice = sewingPrice;
            Description = description;
        }

        public void Update(string name, decimal sewingPrice, string? description)
        {
            if (sewingPrice < 0) throw new ArgumentException("Цена пошива не может быть отрицательной.");
            Name = name;
            SewingPrice = sewingPrice;
            Description = description;
        }

    }
}