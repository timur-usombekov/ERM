using ERM.Application.DTOs;
using ERM.Application.Interfaces.Data;
using ERM.Application.Interfaces.Services;
using ERM.Application.Mappers;
using ERM.Core.Domain.Entities;
using ERM.Core.Domain.Entities.Enum;
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
                .Include(b => b.Items).ThenInclude(i => i.ClothingModel)
                .Include(b => b.Items).ThenInclude(i => i.FabricColor)
                .AsNoTracking().OrderByDescending(b => b.Date).ToListAsync(ct);

            return batches.Select(b => b.ToDto()).ToList();
        }
        public async Task<CutBatchDto> CreateAsync(string title, DateOnly date, int declaredQuantity, Guid cutterEmployeeId, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var batch = new CutBatch(title, date, declaredQuantity, cutterEmployeeId);
            context.CutBatches.Add(batch);

            await context.SaveChangesAsync(ct);
            return batch.ToDto();
        }

        public async Task<CutBatchItemDto> AddItemToBatchAsync(Guid batchId, Guid modelId, Guid colorId, int quantity, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var batch = await context.CutBatches
                .Include(b => b.Items)
                .Include(b => b.CutterEmployee).ThenInclude(e => e.Cutter) // Инклудим закройщика
                .FirstOrDefaultAsync(b => b.Id == batchId, ct)
                ?? throw new InvalidOperationException($"Крой с Id {batchId} не найден.");

            var model = await context.ClothingModels.FirstOrDefaultAsync(m => m.Id == modelId, ct)
                ?? throw new InvalidOperationException("Модель не найдена.");

            if (batch.CutterEmployee.Cutter == null)
                throw new InvalidOperationException("Выбранный сотрудник больше не является закройщиком.");

            decimal cutPrice = model.SewingPrice * (batch.CutterEmployee.Cutter.Percentage / 100m);

            var (isNewItem, currentItem) = batch.AddItem(modelId, colorId, quantity, cutPrice);

            if (isNewItem) context.CutBatchItems.Entry(currentItem).State = EntityState.Added;

            var assignment = new WorkAssignment(
                batch.CutterEmployeeId,
                OperationType.Cutting,
                quantity,
                cutPrice,
                currentItem.Id, 
                null);          // Размер при раскрое не важен

            context.WorkAssignments.Add(assignment);

            await context.SaveChangesAsync(ct);

            return currentItem.ToDto();
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

        public async Task ToggleStatusAsync(Guid id, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            var batch = await context.CutBatches.FirstOrDefaultAsync(b => b.Id == id, ct)
                ?? throw new InvalidOperationException($"Крой с Id {id} не найден.");

            if (batch.IsClosed) batch.Open();
            else batch.Close();

            await context.SaveChangesAsync(ct);
        }


        public async Task EditItemInBatchAsync(Guid itemId, Guid modelId, Guid fabricColorId, int newQuantity, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var item = await context.CutBatchItems
                .Include(i => i.CutBatch).ThenInclude(b => b.CutterEmployee).ThenInclude(e => e.Cutter)
                .FirstOrDefaultAsync(i => i.Id == itemId, ct)
                ?? throw new InvalidOperationException("Позиция кроя не найдена.");

            var model = await context.ClothingModels.FirstOrDefaultAsync(m => m.Id == modelId, ct)
                ?? throw new InvalidOperationException("Модель одежды не найдена.");

            if (item.CutBatch.CutterEmployee.Cutter == null)
                throw new InvalidOperationException("Сотрудник больше не является закройщиком.");

            int otherItemsSum = item.CutBatch.Items.Where(i => i.Id != itemId).Sum(i => i.Quantity);
            if (otherItemsSum + newQuantity > item.CutBatch.DeclaredQuantity)
                throw new InvalidOperationException("Сумма позиций превышает заявленное количество документа.");

            decimal newCutPrice = model.SewingPrice * (item.CutBatch.CutterEmployee.Cutter.Percentage / 100m);

            item.UpdateDetails(modelId, fabricColorId, newQuantity, newCutPrice);

            // Не забыть пересчитать и обновить начисление закройщику зп
            var cutterAssignment = await context.WorkAssignments
                .FirstOrDefaultAsync(a => a.CutBatchItemId == itemId && a.OperationType == OperationType.Cutting, ct);

            if (cutterAssignment != null)
            {
                cutterAssignment.UpdateQuantity(newQuantity);
                // PricePerUnit обновится неявно через Reflection по идее
            }

            await context.SaveChangesAsync(ct);
        }

        public async Task RemoveItemFromBatchAsync(Guid itemId, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var item = await context.CutBatchItems.FirstOrDefaultAsync(i => i.Id == itemId, ct)
                ?? throw new InvalidOperationException("Позиция кроя не найдена.");

            if (item.IssuedQuantity > 0)
                throw new InvalidOperationException($"Нельзя удалить позицию. Уже выдано {item.IssuedQuantity} шт.");

            // Удалить начисление закройщику
            var cutterAssignments = await context.WorkAssignments
                .Where(a => a.CutBatchItemId == itemId && a.OperationType == OperationType.Cutting)
                .ToListAsync(ct);

            context.WorkAssignments.RemoveRange(cutterAssignments);

            context.CutBatchItems.Remove(item);

            await context.SaveChangesAsync(ct);
        }

        public async Task<CutBatchDto> EditBatchAsync(Guid batchId, string title, DateOnly date, int declaredQuantity, Guid cutterEmployeeId, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var batch = await context.CutBatches
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.Id == batchId, ct)
                ?? throw new InvalidOperationException("Документ не найден.");

            batch.UpdateDetails(title, date, declaredQuantity, cutterEmployeeId);

            await context.SaveChangesAsync(ct);

            return batch.ToDto();
        }

    }
}