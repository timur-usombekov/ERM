using ERM.Application.DTOs;

namespace ERM.Application.Interfaces.Services
{
    public interface ICutBatchService
    {
        Task<IReadOnlyList<CutBatchDto>> GetAllAsync(CancellationToken ct = default);
        Task<CutBatchDto> CreateAsync(string title, DateOnly date, int declaredQuantity, CancellationToken ct = default);
        Task AddItemToBatchAsync(Guid batchId, Guid modelId, Guid fabricColorId, int quantity, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}