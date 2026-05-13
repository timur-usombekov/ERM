using ERM.Core.Domain.Entities.Base;

namespace ERM.Core.Domain.Entities
{
    public class FabricColor : Identity
    {
        public string Name { get; private set; } = null!;

        protected FabricColor() { }
        public FabricColor(string name) => Name = name.Trim();
    }
}