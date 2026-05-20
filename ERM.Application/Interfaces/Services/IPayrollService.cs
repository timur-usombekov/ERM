using ERM.Application.DTOs;

namespace ERM.Application.Interfaces.Services
{
    public interface IPayrollService
    {
        Task<IReadOnlyList<EmployeePayrollDto>> GetPayrollAsync(DateOnly startDate, DateOnly endDate, CancellationToken ct = default);
        Task AddAdjustmentAsync(Guid employeeId, DateOnly date, decimal amount, string reason, CancellationToken ct = default);
        Task PaySalaryAsync(Guid employeeId, DateOnly date, decimal amount, CancellationToken ct = default);
    }

}
