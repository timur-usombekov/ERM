using ERM.Application.Interfaces.Repositories;
using ERM.Application.Interfaces.Services;
using ERM.Core.Domain.Entities;

namespace ERM.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct = default)
            => _repository.GetAllAsync(ct);

        public Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _repository.GetByIdAsync(id, ct);

        public async Task<Employee> CreateAsync(
            string fullName,
            string phoneNumber,
            string? notes = null,
            string? machineNumber = null,
            CancellationToken ct = default)
        {
            var employee = new Employee(fullName, phoneNumber);

            if (!string.IsNullOrWhiteSpace(notes))
                employee.UpdateNotes(notes);

            if (!string.IsNullOrWhiteSpace(machineNumber))
                employee.AssignSeamstressRole(machineNumber); // домен создаёт Seamstress

            await _repository.AddAsync(employee, ct); // один SaveChanges — INSERT Employee + Seamstress
            return employee;
        }

        public async Task<Employee> EditEmployeeAsync(
            Guid id,
            string fullName,
            string phoneNumber,
            string? notes,
            bool isSeamstress,
            string? machineNumber,
            CancellationToken ct = default)
        {
            var employee = await GetOrThrowAsync(id, ct);

            employee.UpdateContacts(fullName, phoneNumber);
            employee.UpdateNotes(notes);

            if (!employee.IsSeamstress && isSeamstress)
            {
                employee.AssignSeamstressRole(machineNumber!);
                _repository.TrackAsNew(employee.Seamstress!);
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

            await _repository.UpdateAsync(employee, ct);

            return employee;
        }

        public async Task<Employee> AssignSeamstressRoleAsync(Guid id, string machineNumber, CancellationToken ct = default)
        {
            var employee = await GetOrThrowAsync(id, ct);
            employee.AssignSeamstressRole(machineNumber); // логика в домене
            await _repository.UpdateAsync(employee, ct);
            return employee;
        }

        public async Task<Employee> RevokeSeamstressRoleAsync(Guid id, CancellationToken ct = default)
        {
            var employee = await GetOrThrowAsync(id, ct);
            employee.RevokeSeamstressRole(); // логика в домене
            await _repository.UpdateAsync(employee, ct);
            return employee;
        }

        public async Task<Employee> UpdateMachineNumberAsync(Guid id, string machineNumber, CancellationToken ct = default)
        {
            var employee = await GetOrThrowAsync(id, ct);
            employee.UpdateMachineNumber(machineNumber); // логика в домене
            await _repository.UpdateAsync(employee, ct);
            return employee;
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
            => await _repository.DeleteAsync(id, ct);

        //  приватный хелпер шоб не дублировать "?? throw"
        private async Task<Employee> GetOrThrowAsync(Guid id, CancellationToken ct)
            => await _repository.GetByIdAsync(id, ct)
               ?? throw new InvalidOperationException($"Сотрудник с Id {id} не найден.");
    }
}