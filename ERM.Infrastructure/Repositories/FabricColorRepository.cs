using ERM.Application.Interfaces.Repositories;
using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERM.Infrastructure.Repositories
{
    public class FabricColorRepository: IFabricColorRepository
    {
        private readonly AppDbContext _context;

        public FabricColorRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<FabricColor>> GetAllAsync(CancellationToken ct = default)
            => await _context.FabricColors
                .AsNoTracking()
                .ToListAsync(ct);

        public async Task<FabricColor?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.FabricColors
                .FirstOrDefaultAsync(c => c.Id == id, ct);

        public async Task AddAsync(FabricColor color, CancellationToken ct = default)
        {
            await _context.FabricColors.AddAsync(color, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var model = await _context.FabricColors.FindAsync([id], ct);
            if (model is null) return;

            _context.FabricColors.Remove(model);
            await _context.SaveChangesAsync(ct);
        }
    }
}
