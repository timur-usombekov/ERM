using ERM.Application.Interfaces.Repositories;
using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERM.Infrastructure.Repositories
{
    public class ClothingModelRepository : IClothingModelRepository
    {
        private readonly AppDbContext _context;

        public ClothingModelRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<ClothingModel>> GetAllAsync(CancellationToken ct = default)
            => await _context.ClothingModels
                .AsNoTracking()
                .ToListAsync(ct);

        public async Task<ClothingModel?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.ClothingModels
                .FirstOrDefaultAsync(m => m.Id == id, ct);

        public async Task AddAsync(ClothingModel model, CancellationToken ct = default)
        {
            await _context.ClothingModels.AddAsync(model, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(ClothingModel model, CancellationToken ct = default)
        {
            if (_context.Entry(model).State == EntityState.Detached)
                _context.ClothingModels.Update(model);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var model = await _context.ClothingModels.FindAsync([id], ct);
            if (model is null) return;

            _context.ClothingModels.Remove(model);
            await _context.SaveChangesAsync(ct);
        }
    }
}
