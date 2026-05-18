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
                .Include(a => a.CutBatchItem)
                    .ThenInclude(i => i.ClothingModel)
                .Include(a => a.CutBatchItem)
                    .ThenInclude(i => i.FabricColor)
                .Where(a => a.AssignedDate == today && a.OperationType == OperationType.Sewing)
                .AsNoTracking()
                .ToListAsync(ct);
            return assignments.Select(a => a.ToDto()).OrderByDescending(a => a.AssignedDate).ToList();
        }

        public async Task<WorkAssignmentDto> IssueWorkAsync(Guid employeeId, OperationType operationType, Guid? cutBatchItemId, string? size, int quantity, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            decimal price = 0;

            var employee = await context.Employees
                .Include(e => e.Seamstress)
                .FirstOrDefaultAsync(e => e.Id == employeeId, ct)
                ?? throw new InvalidOperationException("Сотрудник не найден.");

            CutBatchItem? cutItem = null;

            if (cutBatchItemId.HasValue)
            {
                cutItem = await context.CutBatchItems
                    .Include(c => c.ClothingModel)
                    .FirstOrDefaultAsync(c => c.Id == cutBatchItemId.Value, ct)
                    ?? throw new InvalidOperationException("Партия кроя не найдена.");

                cutItem.Issue(quantity);
                price = cutItem.ClothingModel.SewingPrice; // Пока берем цену пошива
            }

            var assignment = new WorkAssignment(employeeId, operationType, quantity, price, cutBatchItemId, size);
            context.WorkAssignments.Add(assignment);
            await context.SaveChangesAsync(ct);

            return new WorkAssignmentDto
            {
                Id = assignment.Id,
                EmployeeName = employee.FullName,
                MachineNumber = employee.Seamstress?.MachineNumber ?? "?",
                ClothingModelName = cutItem?.ClothingModel?.Name ?? "-",
                Color = cutItem?.FabricColor?.Name ?? "-",
                Size = size ?? "-",
                Quantity = quantity,
                AssignedDate = assignment.AssignedDate
            };

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


        public async Task<IReadOnlyList<EmployeePayrollDto>> GetPayrollAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var assignments = await context.WorkAssignments
                .Include(a => a.Employee).ThenInclude(e => e.Seamstress)
                .Include(a => a.Employee).ThenInclude(e => e.Cutter)
                .Include(a => a.CutBatchItem).ThenInclude(i => i.ClothingModel)
                .Where(a => a.AssignedDate >= startDate && a.AssignedDate <= endDate)
                .AsNoTracking().ToListAsync(ct);

            var adjustments = await context.PayrollAdjustments
                .Include(a => a.Employee).ThenInclude(e => e.Seamstress)
                .Include(a => a.Employee).ThenInclude(e => e.Cutter)
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .AsNoTracking().ToListAsync(ct);

            var allEmployeeIds = assignments.Select(a => a.EmployeeId)
                .Union(adjustments.Select(a => a.EmployeeId)).Distinct();

            var result = new List<EmployeePayrollDto>();

            foreach (var empId in allEmployeeIds)
            {
                var empAssignments = assignments.Where(a => a.EmployeeId == empId).ToList();
                var empAdjustments = adjustments.Where(a => a.EmployeeId == empId).ToList();
                var emp = empAssignments.FirstOrDefault()?.Employee ?? empAdjustments.FirstOrDefault()?.Employee;

                var dto = new EmployeePayrollDto
                {
                    EmployeeId = empId,
                    EmployeeName = emp?.FullName ?? "Неизвестно",
                    MachineNumber = emp?.Seamstress?.MachineNumber ?? "?",

                    IsSeamstress = emp?.IsSeamstress ?? false,
                    IsCutter = emp?.IsCutter ?? false,

                    TotalItemsProcessed = empAssignments.Sum(a => a.Quantity),
                    EarnedByOperations = empAssignments.Sum(a => a.TotalPrice),
                    TotalAdjustments = empAdjustments.Sum(a => a.Amount),

                    OperationDetails = empAssignments
                        .GroupBy(a => new { Name = a.CutBatchItem?.ClothingModel?.Name ?? "Прочее", a.PricePerUnit })
                        .Select(g => new PayrollDetailDto
                        {
                            ModelName = g.Key.Name,
                            PricePerUnit = g.Key.PricePerUnit,
                            Quantity = g.Sum(x => x.Quantity)
                        }).OrderBy(d => d.ModelName).ToList(),

                    AdjustmentDetails = empAdjustments
                        .Select(a => new AdjustmentDetailDto { Id = a.Id, Date = a.Date, Amount = a.Amount, Reason = a.Reason })
                        .OrderBy(a => a.Date).ToList()
                };
                result.Add(dto);
            }
            return result.OrderBy(p => p.EmployeeName).ToList();
        }


        public async Task AddAdjustmentAsync(Guid employeeId, DateOnly date, decimal amount, string reason, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            context.PayrollAdjustments.Add(new PayrollAdjustment(employeeId, date, amount, reason));
            await context.SaveChangesAsync(ct);
        }

        public async Task PaySalaryAsync(Guid employeeId, DateOnly date, decimal amount, CancellationToken ct = default)
        {
            if (amount <= 0) throw new ArgumentException("Сумма к выплате должна быть больше нуля.");
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            context.PayrollAdjustments.Add(new PayrollAdjustment(employeeId, date, -amount, "Выплата ЗП (Закрытие периода)"));
            await context.SaveChangesAsync(ct);
        }
    }
}