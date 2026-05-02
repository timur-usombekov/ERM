namespace ERM.Core.Domain.Entities.Base
{
    public abstract class Identity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

    }
}
