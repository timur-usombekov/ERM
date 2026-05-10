using ERM.Application.DTOs;
using ERM.Application.Interfaces.Repositories;
using ERM.Application.Interfaces.Services;
using ERM.Application.Mappers;
using ERM.Core.Domain.Entities;

namespace ERM.Application.Services
{
    public class WorkAssignmentService : IWorkAssignmentService
    {
        private readonly IWorkAssignmentRepository _repo;

        public WorkAssignmentService(IWorkAssignmentRepository repo) => _repo = repo;

        public async Task<IReadOnlyList<WorkAssignmentDto>> GetTodayAssignmentsAsync(CancellationToken ct = default)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var assignments = await _repo.GetByDateAsync(today, ct);

            return assignments.Select(a => a.ToDto()).OrderByDescending(a => a.AssignedDate).ToList();
        }

        public async Task<WorkAssignmentDto> IssueWorkAsync(Guid seamstressId, Guid cutBatchItemId, string size, int quantity, CancellationToken ct = default)
        {
            var assignment = new WorkAssignment(seamstressId, cutBatchItemId, size, quantity);

            await _repo.AddAsync(assignment, ct);

            return assignment.ToDto();
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
            => await _repo.DeleteAsync(id, ct);
    }
}