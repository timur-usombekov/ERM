using ERM.Application.DTOs;
using ERM.Application.Interfaces.Data;
using ERM.Application.Interfaces.Services;
using ERM.Core.Domain.Entities;
using ERM.Core.Domain.Entities.Enum;
using Microsoft.EntityFrameworkCore;

namespace ERM.Application.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IAppDbContextFactory _contextFactory;

        public PayrollService(IAppDbContextFactory contextFactory) => _contextFactory = contextFactory;

        public async Task<IReadOnlyList<EmployeePayrollDto>> GetPayrollAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var allAssignments = await context.WorkAssignments
                .Include(a => a.Employee)
                .Include(a => a.CutBatchItem).ThenInclude(i => i.ClothingModel)
                .Where(a => a.AssignedDate >= startDate && a.AssignedDate <= endDate)
                .AsNoTracking()
                .ToListAsync(ct);

            var adjustments = await context.PayrollAdjustments
                .Include(a => a.Employee)
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .AsNoTracking()
                .ToListAsync(ct);

            var allEmployeeIds = allAssignments.Select(a => a.EmployeeId)
                .Union(adjustments.Select(a => a.EmployeeId))
                .Distinct();

            if (!allEmployeeIds.Any())
                return new List<EmployeePayrollDto>();

            var employeesDict = await context.Employees
                .Include(e => e.Seamstress)
                .Include(e => e.Cutter)
                .Include(e => e.Ironer)
                .Where(e => allEmployeeIds.Contains(e.Id))
                .AsNoTracking()
                .ToDictionaryAsync(e => e.Id, ct);

            var result = new List<EmployeePayrollDto>();

            foreach (var empId in allEmployeeIds)
            {
                if (!employeesDict.TryGetValue(empId, out var emp)) continue;

                var empWorks = allAssignments.Where(a => a.EmployeeId == empId).ToList();
                var empAdjs = adjustments.Where(a => a.EmployeeId == empId).ToList();

                if (emp == null) continue;

                result.Add(new EmployeePayrollDto
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.FullName,

                    IsSeamstress = emp.IsSeamstress,
                    MachineNumber = emp.Seamstress?.MachineNumber ?? string.Empty,

                    IsCutter = emp.IsCutter,
                    IsIroner = emp.IsIroner,

                    TotalItemsProcessed = empWorks.Sum(a =>
                        emp.IsSeamstress
                        ? (a.OperationType == OperationType.Sewing ? a.Quantity : 0)
                        : a.Quantity),
                    EarnedByOperations = empWorks.Sum(a => a.TotalPrice),
                    TotalAdjustments = empAdjs.Sum(a => a.Amount),

                    // Группируем по Операции + Модели
                    OperationDetails = empWorks
                        .GroupBy(a => new { Op = a.OperationType, Model = a.CutBatchItem?.ClothingModel?.Name ?? "Прочее", a.PricePerUnit })
                        .Select(g => new PayrollDetailDto
                        {
                            OperationName = TranslateOperation(g.Key.Op), // Метод типа: Sewing -> "Пошив"
                            ModelName = g.Key.Model,
                            PricePerUnit = g.Key.PricePerUnit,
                            Quantity = g.Sum(x => x.Quantity)
                        })
                        .OrderBy(d => d.OperationName).ThenBy(d => d.ModelName)
                        .ToList(),

                    AdjustmentDetails = empAdjs
                        .Select(a => new AdjustmentDetailDto { Id = a.Id, Date = a.Date, Amount = a.Amount, Reason = a.Reason })
                        .OrderBy(a => a.Date).ToList()
                });
            }
            return result;
        }

        private string TranslateOperation(OperationType type) => type switch
        {
            OperationType.Sewing => "Пошив",
            OperationType.Cutting => "Раскрой",
            OperationType.Ironing => "ВТО / Глажка",
            OperationType.Extra => "Доп. работы",
            _ => "Прочее"
        };

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
