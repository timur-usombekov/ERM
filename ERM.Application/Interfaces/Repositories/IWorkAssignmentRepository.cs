using ERM.Core.Domain.Entities;

namespace ERM.Application.Interfaces.Repositories
{
    public interface IWorkAssignmentRepository
    {
        Task<WorkAssignment?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<WorkAssignment>> GetByDateAsync(DateOnly date, CancellationToken ct = default);
        Task AddAsync(WorkAssignment assignment, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}