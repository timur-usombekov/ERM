using ERM.Core.Domain.Entities;

namespace ERM.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct = default);
        Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Employee> CreateAsync(string fullName, string phoneNumber, string? notes = null, string? machineNumber = null, CancellationToken ct = default);
        Task<Employee> EditEmployeeAsync(
            Guid id,
            string fullName,
            string phoneNumber,
            string? notes,
            bool isSeamstress,
            string? machineNumber,
            CancellationToken ct = default);
        Task<Employee> AssignSeamstressRoleAsync(Guid id, string machineNumber, CancellationToken ct = default);
        Task<Employee> RevokeSeamstressRoleAsync(Guid id, CancellationToken ct = default);
        Task<Employee> UpdateMachineNumberAsync(Guid id, string machineNumber, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}