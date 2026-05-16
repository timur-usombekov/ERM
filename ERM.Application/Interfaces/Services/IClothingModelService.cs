using ERM.Application.DTOs;

namespace ERM.Application.Interfaces.Services
{
    public interface IClothingModelService
    {
        Task<IReadOnlyList<ClothingModelDto>> GetAllAsync(CancellationToken ct = default);
        Task<ClothingModelDto> CreateAsync(string name, decimal sewingPrice, string? description, CancellationToken ct = default);
        Task<ClothingModelDto> EditAsync(Guid id, string name, decimal sewingPrice, string? description, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
