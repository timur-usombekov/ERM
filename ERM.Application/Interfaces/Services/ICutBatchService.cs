using ERM.Application.DTOs;

namespace ERM.Application.Interfaces.Services
{
    public interface ICutBatchService
    {
        Task<IReadOnlyList<CutBatchDto>> GetAllAsync(CancellationToken ct = default);
        Task<CutBatchDto> CreateAsync(string title, DateOnly date, int declaredQuantity, Guid cutterEmployeeId, CancellationToken ct = default);
        Task<CutBatchItemDto> AddItemToBatchAsync(Guid batchId, Guid modelId, Guid fabricColorId, int quantity, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task ToggleStatusAsync(Guid id, CancellationToken ct = default);

        Task EditItemInBatchAsync(Guid itemId, Guid modelId, Guid fabricColorId, int newQuantity, CancellationToken ct = default);
        Task RemoveItemFromBatchAsync(Guid itemId, CancellationToken ct = default);


    }
}