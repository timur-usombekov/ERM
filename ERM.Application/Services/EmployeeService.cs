using ERM.Application.Interfaces.Data;
using ERM.Application.Interfaces.Services;
using ERM.Core.Domain.Entities;
using ERM.Application.Mappers;
using ERM.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ERM.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IAppDbContextFactory _contextFactory;

        public EmployeeService(IAppDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            var employees = await context.Employees
                .Include(e => e.Seamstress)
                .Include(e => e.Cutter)
                .AsNoTracking().ToListAsync(ct);

            return employees.Select(e => e.ToDto()).ToList();
        }

        public async Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            var employee = await context.Employees
                .Include(e => e.Seamstress)
                .Include(e => e.Cutter)
                .FirstOrDefaultAsync(e => e.Id == id, ct);
            if (employee is null)
                throw new InvalidOperationException($"Сотрудник с Id {id} не найден.");
            
            return employee?.ToDto();
        }
        public async Task<EmployeeDto> CreateAsync(string fullName, string phoneNumber, string? notes = null,
            bool isSeamstress = false, string? machineNumber = null,
            bool isCutter = false, decimal? cutterPercentage = null, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            var employee = new Employee(fullName, phoneNumber);

            if (!string.IsNullOrWhiteSpace(notes)) employee.UpdateGeneralInfo(fullName, phoneNumber, notes);

            if (isSeamstress && !string.IsNullOrWhiteSpace(machineNumber))
                employee.AssignSeamstressRole(machineNumber);

            if (isCutter && cutterPercentage.HasValue)
                employee.AssignCutterRole(cutterPercentage.Value);

            context.Employees.Add(employee);
            await context.SaveChangesAsync(ct);
            return employee.ToDto();
        }

        public async Task<EmployeeDto> EditEmployeeAsync(Guid id, string fullName, string phoneNumber, string? notes,
            bool isSeamstress, string? machineNumber,
            bool isCutter, decimal? cutterPercentage, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var employee = await context.Employees
                .Include(e => e.Seamstress)
                .Include(e => e.Cutter)
                .FirstOrDefaultAsync(e => e.Id == id, ct)
                ?? throw new InvalidOperationException("Сотрудник не найден.");

            employee.UpdateGeneralInfo(fullName, phoneNumber, notes);

            if (!employee.IsSeamstress && isSeamstress)
            {
                employee.AssignSeamstressRole(machineNumber!);
                context.Seamstresses.Add(employee.Seamstress!); // домен уже создал Seamstress, нужно только добавить в контекст
            }
            else if (employee.IsSeamstress && !isSeamstress)
            {
                // пока вроде бд удаляет Seamstress и при Seamstress = null
                // _repository.RemoveSeamstress(employee.Seamstress!);
                employee.RevokeSeamstressRole();
            }
            else if (employee.IsSeamstress && isSeamstress && employee.Seamstress!.MachineNumber != machineNumber)
            {
                employee.UpdateMachineNumber(machineNumber!);
            }

            if (!employee.IsCutter && isCutter) 
            { 
                employee.AssignCutterRole(cutterPercentage!.Value); 
                context.Cutters.Add(employee.Cutter!); 
            }
            else if (employee.IsCutter && !isCutter) 
            { 
                employee.RevokeCutterRole(); 
            }
            else if (employee.IsCutter && isCutter && employee.Cutter!.Percentage != cutterPercentage) 
            { 
                employee.UpdateCutterPercentage(cutterPercentage!.Value); 
            }
            await context.SaveChangesAsync(ct);

            return employee.ToDto();
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);
            if (employee is null)
                throw new InvalidOperationException($"Сотрудник с Id {id} не найден.");
            context.Employees.Remove(employee);
            await context.SaveChangesAsync(ct);
        }
    }
}