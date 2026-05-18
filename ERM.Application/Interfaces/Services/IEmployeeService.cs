using ERM.Application.DTOs;

namespace ERM.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken ct = default);
        Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<EmployeeDto> CreateAsync(string fullName, string phoneNumber, string? notes = null,
            bool isSeamstress = false, string? machineNumber = null,
            bool isCutter = false, decimal? cutterPercentage = null, CancellationToken ct = default);

        Task<EmployeeDto> EditEmployeeAsync(Guid id, string fullName, string phoneNumber, string? notes,
            bool isSeamstress, string? machineNumber,
            bool isCutter, decimal? cutterPercentage, CancellationToken ct = default);

        Task DeleteAsync(Guid id, CancellationToken ct = default);

    }
}