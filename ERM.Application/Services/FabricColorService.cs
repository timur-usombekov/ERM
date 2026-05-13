using ERM.Application.DTOs;
using ERM.Application.Mappers;
using ERM.Application.Interfaces.Data;
using ERM.Application.Interfaces.Services;
using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERM.Application.Services
{
    public class FabricColorService: IFabricColorService
    {
        private readonly IAppDbContextFactory _contextFactory;

        public FabricColorService(IAppDbContextFactory contextFactory) => _contextFactory = contextFactory;

        public async Task<IReadOnlyList<FabricColorDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            var colors = await context.FabricColors.AsNoTracking().ToListAsync(ct);
            return colors.Select(c => c.ToDto()).ToList();
        }

        public async Task<FabricColorDto> GetOrCreateAsync(string name, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var cleanName = name.Trim();

            var allColors = await context.FabricColors.AsNoTracking().ToListAsync(ct);
            var existing = allColors.FirstOrDefault(c => c.Name.ToLower() == cleanName.ToLower());

            if (existing is not null)
                return new FabricColorDto { Id = existing.Id, Name = existing.Name };

            var newColor = new FabricColor(cleanName);
            context.FabricColors.Add(newColor);

            await context.SaveChangesAsync(ct);

            return new FabricColorDto { Id = newColor.Id, Name = newColor.Name };
        }


        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            var color = await context.FabricColors.FirstOrDefaultAsync(c => c.Id == id, ct);
            if (color is null) return;
            context.FabricColors.Remove(color);
            await context.SaveChangesAsync(ct);
        }
        
    }
}
