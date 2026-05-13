using ERM.Application.DTOs;
using ERM.Application.Interfaces.Repositories;
using ERM.Application.Interfaces.Services;
using ERM.Application.Mappers;
using ERM.Core.Domain.Entities;

namespace ERM.Application.Services
{
    public class CutBatchService : ICutBatchService
    {
        private readonly ICutBatchRepository _repo;

        public CutBatchService(ICutBatchRepository repo) => _repo = repo;

        public async Task<IReadOnlyList<CutBatchDto>> GetAllAsync(CancellationToken ct = default)
        {
            var batches = await _repo.GetAllAsync(ct);
            return batches.Select(b => b.ToDto()).ToList();
        }

        public async Task<CutBatchDto> CreateAsync(string title, DateOnly date, int declaredQuantity, CancellationToken ct = default)
        {
            var batch = new CutBatch(title, date, declaredQuantity);
            await _repo.AddAsync(batch, ct);
            return batch.ToDto();
        }


        public async Task AddItemToBatchAsync(Guid batchId, Guid modelId, Guid colorId, int quantity, CancellationToken ct = default)
        {
            var batch = await GetOrThrowAsync(batchId, ct);

            batch.AddItem(modelId, colorId, quantity);
            _repo.TrackAsNew(batch.Items.Last());
            await _repo.UpdateAsync(batch, ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
            => await _repo.DeleteAsync(id, ct);

        private async Task<CutBatch> GetOrThrowAsync(Guid id, CancellationToken ct)
            => await _repo.GetByIdAsync(id, ct)
               ?? throw new InvalidOperationException($"Крой с Id {id} не найден.");
    }
}