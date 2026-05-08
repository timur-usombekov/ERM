using ERM.Core.Domain.Entities;

namespace ERM.Application.Interfaces.Repositories
{
    public interface IClothingModelRepository
    {
        Task<IReadOnlyList<ClothingModel>> GetAllAsync(CancellationToken ct = default);
        Task<ClothingModel?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(ClothingModel model, CancellationToken ct = default);
        Task UpdateAsync(ClothingModel model, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
