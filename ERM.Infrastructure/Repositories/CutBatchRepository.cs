using ERM.Application.Interfaces.Repositories;
using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERM.Infrastructure.Repositories
{
    public class CutBatchRepository : ICutBatchRepository
    {
        private readonly AppDbContext _context;

        public CutBatchRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<CutBatch>> GetAllAsync(CancellationToken ct = default)
            => await _context.CutBatches
                .Include(b => b.Items)
                    .ThenInclude(i => i.ClothingModel)
                .AsNoTracking()
                .OrderByDescending(b => b.Date)
                .ToListAsync(ct);

        public async Task<CutBatch?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.CutBatches
                .Include(b => b.Items)
                    .ThenInclude(i => i.ClothingModel)
                .FirstOrDefaultAsync(b => b.Id == id, ct);

        public async Task AddAsync(CutBatch cutBatch, CancellationToken ct = default)
        {
            await _context.CutBatches.AddAsync(cutBatch, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(CutBatch cutBatch, CancellationToken ct = default)
        {
            if (_context.Entry(cutBatch).State == EntityState.Detached)
                _context.CutBatches.Update(cutBatch);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var batch = await _context.CutBatches.FindAsync([id], ct);
            if (batch is null) return;

            _context.CutBatches.Remove(batch);
            await _context.SaveChangesAsync(ct);
        }

        public void TrackAsNew(CutBatchItem cutBatchItem)
        {
            _context.CutBatchItems.Add(cutBatchItem); // явно говорим EF: это INSERT, не UPDATE
        }
    }
}