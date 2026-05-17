using ERM.Application.DTOs;

namespace ERM.Application.Interfaces.Services
{
    public interface IWorkAssignmentService
    {
        Task<IReadOnlyList<WorkAssignmentDto>> GetTodayAssignmentsAsync(CancellationToken ct = default);
        Task<WorkAssignmentDto> IssueWorkAsync(Guid seamstressId, Guid cutBatchItemId, string size, int quantity, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<SeamstressPayrollDto>> GetPayrollAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct = default);
        Task AddAdjustmentAsync(Guid seamstressId, DateOnly date, decimal amount, string reason, CancellationToken ct = default);
        Task PaySalaryAsync(Guid seamstressId, DateOnly date, decimal amount, CancellationToken ct = default);

    }
}