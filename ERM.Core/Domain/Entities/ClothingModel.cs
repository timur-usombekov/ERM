using ERM.Core.Domain.Entities.Base;

namespace ERM.Core.Domain.Entities
{
    public class ClothingModel : Identity
    {
        public string Name { get; private set; } = null!;
        public string Article { get; private set; } = null!;
        public decimal SewingPrice { get; private set; }
        public decimal IroningPrice { get; private set; }
        public string? Description { get; private set; }

        public bool IsDeleted { get; private set; }

        protected ClothingModel() { }

        public ClothingModel(string name, string article, decimal sewingPrice, decimal ironingPrice, string? description = null)
        {
            if (sewingPrice < 0) throw new ArgumentException("Цена пошива не может быть отрицательной.");
            if (ironingPrice < 0) throw new ArgumentException("Цена глажки не может быть отрицательной.");
            Name = name;
            Article = article;
            SewingPrice = sewingPrice;
            IroningPrice = ironingPrice;
            Description = description;
        }

        public void Update(string name, string article, decimal sewingPrice, decimal ironingPrice, string? description)
        {
            if (sewingPrice < 0) throw new ArgumentException("Цена пошива не может быть отрицательной.");
            if (ironingPrice < 0) throw new ArgumentException("Цена глажки не может быть отрицательной.");
            Name = name;
            Article = article;
            SewingPrice = sewingPrice;
            IroningPrice = ironingPrice;
            Description = description;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }

    }
}