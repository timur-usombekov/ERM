using ERM.Application.DTOs;
using ERM.Application.Interfaces.Data;
using ERM.Application.Interfaces.Services;
using ERM.Application.Mappers;
using ERM.Core.Domain.Entities;
using ERM.Core.Domain.Entities.Enum;
using Microsoft.EntityFrameworkCore;

namespace ERM.Application.Services
{
    public class WorkAssignmentService : IWorkAssignmentService
    {
        private readonly IAppDbContextFactory _contextFactory;

        public WorkAssignmentService(IAppDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IReadOnlyList<WorkAssignmentDto>> GetTodayAssignmentsAsync(CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var today = DateOnly.FromDateTime(DateTime.Today);
            var assignments = await context.WorkAssignments
                .Include(a => a.Employee)
                    .ThenInclude(s => s.Seamstress)
                .Include(a => a.Employee)
                    .ThenInclude(ir => ir.Ironer)
                .Include(a => a.CutBatchItem)
                    .ThenInclude(i => i.ClothingModel)
                .Include(a => a.CutBatchItem)
                    .ThenInclude(i => i.FabricColor)
                .Where(a => a.AssignedDate == today && a.OperationType == OperationType.Sewing)
                .AsNoTracking()
                .ToListAsync(ct);
            return assignments.Select(a => a.ToDto()).OrderByDescending(a => a.AssignedDate).ToList();
        }

        public async Task IssueSewingAsync(Guid seamstressId, Guid shiftIronerId, Guid cutBatchItemId, string size, int quantity, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var cutItem = await context.CutBatchItems
                .Include(c => c.ClothingModel)
                .FirstOrDefaultAsync(c => c.Id == cutBatchItemId, ct)
                ?? throw new InvalidOperationException("Партия кроя не найдена.");

            // Списывание кроя (это делается только при пошиве)
            cutItem.Issue(quantity);

            var sewingAssignment = new WorkAssignment(
                seamstressId,
                OperationType.Sewing,
                quantity,
                cutItem.ClothingModel.SewingPrice,
                cutBatchItemId,
                size);

            var autoIroningAssignment = new WorkAssignment(
                shiftIronerId,
                OperationType.Ironing, 
                quantity,
                cutItem.ClothingModel.IroningPrice,
                cutBatchItemId,
                null); // Для глажки размер не важен

            context.WorkAssignments.Add(sewingAssignment);
            context.WorkAssignments.Add(autoIroningAssignment);

            await context.SaveChangesAsync(ct);
        }

        public async Task RegisterIroningSubstitutionAsync(Guid substituteEmpId, Guid mainIronerId, Guid cutBatchItemId, int quantity, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var currentIronerBalance = await context.WorkAssignments
                .Where(a => a.EmployeeId == mainIronerId && 
                        a.CutBatchItemId == cutBatchItemId && 
                        a.OperationType == OperationType.Ironing)
                .SumAsync(a => a.Quantity, ct);

            if (quantity > currentIronerBalance)
                throw new InvalidOperationException(
                    $"Невозможно записать {quantity} шт. На гладильщице числится только {currentIronerBalance} отшитых единиц этой модели.");

            var cutItem = await context.CutBatchItems
                .Include(c => c.ClothingModel)
                .FirstOrDefaultAsync(c => c.Id == cutBatchItemId, ct)
                ?? throw new InvalidOperationException("Партия кроя не найдена.");

            var substituteAssignment = new WorkAssignment(
                substituteEmpId,
                OperationType.Ironing,
                quantity,
                cutItem.ClothingModel.IroningPrice,
                cutBatchItemId,
                null);

            var deductionAssignment = new WorkAssignment(
                mainIronerId,
                OperationType.Ironing,
                -quantity,
                cutItem.ClothingModel.IroningPrice,
                cutBatchItemId,
                null);

            context.WorkAssignments.Add(substituteAssignment);
            context.WorkAssignments.Add(deductionAssignment);

            await context.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<CutBatchItemDto>> GetAvailableForSubstitutionAsync(Guid mainIronerId, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            // Bсе назначения глажки для гладильщицы и суммируем их по партиям кроя
            var balances = await context.WorkAssignments
                .Where(a => a.EmployeeId == mainIronerId && a.OperationType == OperationType.Ironing)
                .GroupBy(a => a.CutBatchItemId)
                .Select(g => new { CutBatchItemId = g.Key, Balance = g.Sum(x => x.Quantity) })
                .Where(x => x.Balance > 0) // Берем только то, где баланс > 0
                .ToListAsync(ct);

            if (!balances.Any()) return new List<CutBatchItemDto>();

            var batchItemIds = balances.Select(b => b.CutBatchItemId).ToList();

            var cutItems = await context.CutBatchItems
                .Include(c => c.ClothingModel)
                .Include(c => c.FabricColor)
                .Where(c => batchItemIds.Contains(c.Id))
                .ToListAsync(ct);

            var result = new List<CutBatchItemDto>();
            foreach (var balance in balances)
            {
                var item = cutItems.FirstOrDefault(c => c.Id == balance.CutBatchItemId);
                if (item != null)
                {
                    result.Add(new CutBatchItemDto
                    {
                        Id = item.Id,
                        ClothingModelId = item.ClothingModelId,
                        ClothingModelName = item.ClothingModel.Name,
                        Color = item.FabricColor.Name,
                        Quantity = item.Quantity,

                        AvailableQuantity = balance.Balance
                    });
                }
            }

            return result.OrderBy(r => r.ClothingModelName).ToList();
        }


        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            var assignment = await context.WorkAssignments.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (assignment is null) return;

            if (assignment.CutBatchItemId.HasValue)
            {
                var cutItem = await context.CutBatchItems.FirstOrDefaultAsync(c => c.Id == assignment.CutBatchItemId.Value, ct);
                cutItem?.CancelIssue(assignment.Quantity);
            }

            context.WorkAssignments.Remove(assignment);
            await context.SaveChangesAsync(ct);
        }


    }
}