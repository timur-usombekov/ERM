using ERM.Application.DTOs;

namespace ERM.Application.Interfaces.Services
{
    public interface IWorkAssignmentService
    {
        Task<IReadOnlyList<WorkAssignmentDto>> GetTodayAssignmentsAsync(CancellationToken ct = default);
        Task<WorkAssignmentDto> IssueWorkAsync(Guid seamstressId, Guid cutBatchItemId, string size, int quantity, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}