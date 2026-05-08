using ERM.Application.DTOs;
using ERM.Application.Interfaces.Repositories;
using ERM.Application.Interfaces.Services;
using ERM.Application.Mappers;
using ERM.Core.Domain.Entities;

namespace ERM.Application.Services
{
    public class ClothingModelService : IClothingModelService
    {
        private readonly IClothingModelRepository _repo;

        public ClothingModelService(IClothingModelRepository repo) => _repo = repo;

        public async Task<IReadOnlyList<ClothingModelDto>> GetAllAsync(CancellationToken ct = default)
        {
            var models = await _repo.GetAllAsync(ct);
            return models.Select(m => m.ToDto()).ToList();
        }

        public async Task<ClothingModelDto> CreateAsync(string name, string? description, CancellationToken ct = default)
        {
            var model = new ClothingModel(name, description);
            await _repo.AddAsync(model, ct);
            return model.ToDto();
        }

        public async Task<ClothingModelDto> EditAsync(Guid id, string name, string? description, CancellationToken ct = default)
        {
            var model = await GetOrThrowAsync(id, ct);
            model.Update(name, description);
            await _repo.UpdateAsync(model, ct);
            return model.ToDto();
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
            => await _repo.DeleteAsync(id, ct);

        // Помагашка
        private async Task<ClothingModel> GetOrThrowAsync(Guid id, CancellationToken ct)
            => await _repo.GetByIdAsync(id, ct)
               ?? throw new InvalidOperationException($"Модель одежды с Id {id} не найдена.");
    }
}
