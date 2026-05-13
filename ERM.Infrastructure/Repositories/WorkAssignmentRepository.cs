using ERM.Application.Interfaces.Repositories;
using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERM.Infrastructure.Repositories
{
    public class WorkAssignmentRepository : IWorkAssignmentRepository
    {
        private readonly AppDbContext _context;

        public WorkAssignmentRepository(AppDbContext context) => _context = context;

        public async Task<WorkAssignment?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.WorkAssignments.FirstOrDefaultAsync(a => a.Id == id, ct);

        public async Task<IReadOnlyList<WorkAssignment>> GetByDateAsync(DateOnly date, CancellationToken ct = default)
            => await _context.WorkAssignments
                .Include(a => a.Seamstress)
                    .ThenInclude(s => s.Employee)
                .Include(a => a.CutBatchItem)
                    .ThenInclude(i => i.ClothingModel)
                .Include(a => a.CutBatchItem)
                    .ThenInclude(i => i.FabricColor)
                .Where(a => a.AssignedDate == date)
                .AsNoTracking()
                .ToListAsync(ct);

        public async Task AddAsync(WorkAssignment assignment, CancellationToken ct = default)
        {
            await _context.WorkAssignments.AddAsync(assignment, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var assignment = await _context.WorkAssignments.FindAsync([id], ct);
            if (assignment is null) return;

            _context.WorkAssignments.Remove(assignment);
            await _context.SaveChangesAsync(ct);
        }
    }
}