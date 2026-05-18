using ERM.Application.DTOs;
using ERM.Core.Domain.Entities.Enum;

namespace ERM.Application.Interfaces.Services
{
    public interface IWorkAssignmentService
    {
        Task<IReadOnlyList<WorkAssignmentDto>> GetTodayAssignmentsAsync(CancellationToken ct = default);

        Task<WorkAssignmentDto> IssueWorkAsync(Guid employeeId, OperationType operationType, Guid? cutBatchItemId, string? size, int quantity, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);

        Task<IReadOnlyList<EmployeePayrollDto>> GetPayrollAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct = default);
        Task AddAdjustmentAsync(Guid employeeId, DateOnly date, decimal amount, string reason, CancellationToken ct = default);
        Task PaySalaryAsync(Guid employeeId, DateOnly date, decimal amount, CancellationToken ct = default);


    }
}