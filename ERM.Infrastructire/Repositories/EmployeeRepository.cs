using ERM.Application.Interfaces.Repositories;
using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERM.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Employees
                .Include(e => e.Seamstress)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Employees
                .Include(e => e.Seamstress)
                .FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task AddAsync(Employee employee, CancellationToken ct = default)
        {
            await _context.Employees.AddAsync(employee, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Employee employee, CancellationToken ct = default)
        {
            if (_context.Entry(employee).State == EntityState.Detached)
                _context.Employees.Update(employee);
            await _context.SaveChangesAsync(ct);
        }
        public void TrackAsNew(Seamstress seamstress)
        {
            _context.Seamstresses.Add(seamstress); // явно говорим EF: это INSERT, не UPDATE
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var employee = await _context.Employees.FindAsync([id], ct);
            if (employee is null) return;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync(ct);
        }
    }
}