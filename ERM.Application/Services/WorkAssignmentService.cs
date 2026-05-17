using ERM.Application.DTOs;
using ERM.Application.Interfaces.Data;
using ERM.Application.Interfaces.Services;
using ERM.Application.Mappers;
using ERM.Core.Domain.Entities;
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
                .Include(a => a.Seamstress)
                    .ThenInclude(s => s.Employee)
                .Include(a => a.CutBatchItem)
                    .ThenInclude(i => i.ClothingModel)
                .Include(a => a.CutBatchItem)
                    .ThenInclude(i => i.FabricColor)
                .Where(a => a.AssignedDate == today)
                .AsNoTracking()
                .ToListAsync(ct);
            return assignments.Select(a => a.ToDto()).OrderByDescending(a => a.AssignedDate).ToList();
        }

        public async Task<WorkAssignmentDto> IssueWorkAsync(Guid seamstressId, Guid cutBatchItemId, string size, int quantity, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var cutItem = await context.CutBatchItems
                .Include(c => c.ClothingModel)
                .FirstOrDefaultAsync(c => c.Id == cutBatchItemId, ct)
                ?? throw new InvalidOperationException("Партия кроя не найдена.");

            cutItem.Issue(quantity);

            decimal currentPrice = cutItem.ClothingModel.SewingPrice;

            var assignment = new WorkAssignment(seamstressId, cutBatchItemId, size, quantity, currentPrice);
            context.WorkAssignments.Add(assignment);

            await context.SaveChangesAsync(ct);

            return assignment.ToDto();
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var assignment = await context.WorkAssignments
                .FirstOrDefaultAsync(a => a.Id == id, ct);
            if (assignment is null) return;

            var cutItem = await context.CutBatchItems
                .FirstOrDefaultAsync(c => c.Id == assignment.CutBatchItemId, ct);
            // Возвращаем баланс крою при отмене выдачи
            cutItem?.CancelIssue(assignment.Quantity);

            context.WorkAssignments.Remove(assignment);
            await context.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<SeamstressPayrollDto>> GetPayrollAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            //  сдельный заработок (Пошив)
            var assignments = await context.WorkAssignments
                .Include(a => a.Seamstress).ThenInclude(s => s.Employee)
                .Include(a => a.CutBatchItem).ThenInclude(i => i.ClothingModel)
                .Where(a => a.AssignedDate >= startDate && a.AssignedDate <= endDate)
                .AsNoTracking()
                .ToListAsync(ct);

            //  финансовые корректировки 
            var adjustments = await context.PayrollAdjustments
                .Include(a => a.Seamstress).ThenInclude(s => s.Employee)
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .AsNoTracking()
                .ToListAsync(ct);

            // сбор уникальных швей из обоих списков
            var allSeamstressIds = assignments.Select(a => a.SeamstressId)
                .Union(adjustments.Select(a => a.SeamstressId))
                .Distinct();

            var result = new List<SeamstressPayrollDto>();

            // выглядит страшно, но пока ничего лучше не придумалось для объединения данных по швеям из двух разных источников
            // а ещё читается нормально
            foreach (var seamstressId in allSeamstressIds)
            {
                var seamstressAssignments = assignments.Where(a => a.SeamstressId == seamstressId).ToList();
                var seamstressAdjustments = adjustments.Where(a => a.SeamstressId == seamstressId).ToList();

                var sInfo = seamstressAssignments.FirstOrDefault()?.Seamstress
                            ?? seamstressAdjustments.FirstOrDefault()?.Seamstress;

                var dto = new SeamstressPayrollDto
                {
                    SeamstressId = seamstressId,
                    SeamstressName = sInfo?.Employee.FullName ?? "Неизвестно",
                    MachineNumber = sInfo?.MachineNumber ?? "?",

                    TotalItemsSewn = seamstressAssignments.Sum(a => a.Quantity),
                    EarnedBySewing = seamstressAssignments.Sum(a => a.TotalPrice),

                    TotalAdjustments = seamstressAdjustments.Sum(a => a.Amount),

                    // Группируем сшитое для расшифровки
                    SewingDetails = seamstressAssignments
                        .GroupBy(a => new { a.CutBatchItem.ClothingModel.Name, a.PricePerUnit })
                        .Select(g => new PayrollDetailDto
                        {
                            ModelName = g.Key.Name,
                            PricePerUnit = g.Key.PricePerUnit,
                            Quantity = g.Sum(x => x.Quantity)
                        })
                        .OrderBy(d => d.ModelName).ToList(),

                    // Добавляем расшифровку авансов
                    AdjustmentDetails = seamstressAdjustments
                        .Select(a => new AdjustmentDetailDto
                        {
                            Id = a.Id,
                            Date = a.Date,
                            Amount = a.Amount,
                            Reason = a.Reason
                        })
                        .OrderBy(a => a.Date).ToList()
                };

                result.Add(dto);
            }

            return result.OrderBy(p => p.SeamstressName).ToList();
        }

        public async Task AddAdjustmentAsync(Guid seamstressId, DateOnly date, decimal amount, string reason, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var adjustment = new PayrollAdjustment(seamstressId, date, amount, reason);
            context.PayrollAdjustments.Add(adjustment);

            await context.SaveChangesAsync(ct);
        }

        public async Task PaySalaryAsync(Guid seamstressId, DateOnly date, decimal amount, CancellationToken ct = default)
        {
            if (amount <= 0) throw new ArgumentException("Сумма к выплате должна быть больше нуля.");

            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var adjustment = new PayrollAdjustment(seamstressId, date, -amount, "Выплата ЗП (Закрытие периода)");
            context.PayrollAdjustments.Add(adjustment);

            await context.SaveChangesAsync(ct);
        }


    }
}