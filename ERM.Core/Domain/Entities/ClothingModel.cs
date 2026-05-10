using ERM.Core.Domain.Entities.Base;

namespace ERM.Core.Domain.Entities
{
    public class ClothingModel : Identity
    {
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }

        protected ClothingModel() { }

        public ClothingModel(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }

        public void Update(string name, string? description)
        {
            Name = name;
            Description = description;
        }
    }
}