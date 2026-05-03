using ERM.Core.Domain.Entities;

namespace ERM.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct = default);
        Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(Employee employee, CancellationToken ct = default);
        Task UpdateAsync(Employee employee, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        void TrackAsNew(Seamstress seamstress);

    }
}
