using ERM.Application.DTOs;
using ERM.Core.Domain.Entities.Enum;

namespace ERM.Application.Interfaces.Services
{
    public interface IWorkAssignmentService
    {
        Task<IReadOnlyList<WorkAssignmentDto>> GetTodayAssignmentsAsync(CancellationToken ct = default);

        Task IssueSewingAsync(Guid seamstressId, Guid shiftIronerId, Guid cutBatchItemId, string size, int quantity, CancellationToken ct = default);

        Task RegisterIroningSubstitutionAsync(Guid substituteEmpId, Guid mainIronerId, Guid cutBatchItemId, int quantity, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<CutBatchItemDto>> GetAvailableForSubstitutionAsync(Guid mainIronerId, CancellationToken ct = default);

    }
}