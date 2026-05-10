using ERM.Core.Domain.Entities;

namespace ERM.Application.Interfaces.Repositories
{
    public interface ICutBatchRepository
    {
        Task<IReadOnlyList<CutBatch>> GetAllAsync(CancellationToken ct = default);
        Task<CutBatch?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(CutBatch cutBatch, CancellationToken ct = default);
        Task UpdateAsync(CutBatch cutBatch, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        void TrackAsNew(CutBatchItem cutBatchItem);
    }
}