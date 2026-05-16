using ERM.Application.DTOs;
using ERM.Application.Interfaces.Data;
using ERM.Application.Interfaces.Services;
using ERM.Application.Mappers;
using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERM.Application.Services
{
    public class ClothingModelService : IClothingModelService
    {
        private readonly IAppDbContextFactory _contextFactory;

        public ClothingModelService(IAppDbContextFactory contextFactory) => _contextFactory = contextFactory;

        public async Task<IReadOnlyList<ClothingModelDto>> GetAllAsync(CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            var models = await context.ClothingModels.AsNoTracking().ToListAsync(ct);
            return models.Select(m => m.ToDto()).ToList();
        }

        public async Task<ClothingModelDto> CreateAsync(string name, decimal sewingPrice, string? description, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var model = new ClothingModel(name, sewingPrice, description);

            context.ClothingModels.Add(model);
            await context.SaveChangesAsync(ct);
            return model.ToDto();
        }

        public async Task<ClothingModelDto> EditAsync(Guid id, string name, decimal sewingPrice, string? description, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            var model = await context.ClothingModels.FirstOrDefaultAsync(m => m.Id == id, ct);
            if (model is null)
                throw new InvalidOperationException($"Модель одежды с Id {id} не найдена.");

            model.Update(name, sewingPrice, description);
            context.ClothingModels.Update(model);

            await context.SaveChangesAsync(ct);
            return model.ToDto();
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            var model = await context.ClothingModels.FirstOrDefaultAsync(m => m.Id == id, ct);
            if (model is null)
                throw new InvalidOperationException($"Модель одежды с Id {id} не найдена.");

            context.ClothingModels.Remove(model);
            await context.SaveChangesAsync(ct);
        }
    }
}
