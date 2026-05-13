using ERM.Application.DTOs;
using ERM.Application.Interfaces.Data;
using ERM.Application.Interfaces.Services;
using ERM.Application.Mappers;
using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                .FirstOrDefaultAsync(c => c.Id == cutBatchItemId, ct)
                ?? throw new InvalidOperationException("Партия кроя не найдена.");

            cutItem.Issue(quantity);

            var assignment = new WorkAssignment(seamstressId, cutBatchItemId, size, quantity);
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
    }
}