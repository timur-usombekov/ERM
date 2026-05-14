using ERM.Application.DTOs;
using ERM.Application.Interfaces.Data;
using ERM.Application.Interfaces.Services;
using ERM.Application.Mappers;
using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERM.Application.Services
{
    public class CutBatchService : ICutBatchService
    {
        private readonly IAppDbContextFactory _contextFactory;

        public CutBatchService(IAppDbContextFactory contextFactory) => _contextFactory = contextFactory;

        public async Task<IReadOnlyList<CutBatchDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            var batches = await context.CutBatches
                .Include(b => b.Items)
                    .ThenInclude(i => i.ClothingModel)
                .Include(b => b.Items)
                    .ThenInclude(i => i.FabricColor)
                .AsNoTracking()
                .OrderByDescending(b => b.Date)
                .ToListAsync(ct);

            return batches.Select(b => b.ToDto()).ToList();
        }

        public async Task<CutBatchDto> CreateAsync(string title, DateOnly date, int declaredQuantity, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            var batch = new CutBatch(title, date, declaredQuantity);
            context.CutBatches.Add(batch);
            await context.SaveChangesAsync(ct);
            return batch.ToDto();
        }


        public async Task<CutBatchItemDto> AddItemToBatchAsync(Guid batchId, Guid modelId, Guid colorId, int quantity, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var batch = await context.CutBatches
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.Id == batchId, ct);

            if (batch is null)
                throw new InvalidOperationException($"Крой с Id {batchId} не найден.");

            var isNewItem = batch.AddItem(modelId, colorId, quantity);

            if (isNewItem) // Проверка на добавление нового элемента
                context.CutBatchItems.Entry(batch.Items.Last()).State = EntityState.Added;

            await context.SaveChangesAsync(ct);

            return batch.Items.Last().ToDto();
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            var batch = await context.CutBatches.FirstOrDefaultAsync(b => b.Id == id, ct);
            if (batch is null)
                throw new InvalidOperationException($"Крой с Id {id} не найден.");
            context.CutBatches.Remove(batch);
            await context.SaveChangesAsync(ct);
        }
    }
}