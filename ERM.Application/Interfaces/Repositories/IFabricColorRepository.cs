using ERM.Core.Domain.Entities;

namespace ERM.Application.Interfaces.Repositories
{
    public interface IFabricColorRepository
    {
        Task<IReadOnlyList<FabricColor>> GetAllAsync(CancellationToken ct = default);
        Task<FabricColor?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(FabricColor color, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
