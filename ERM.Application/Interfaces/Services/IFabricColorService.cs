using ERM.Application.DTOs;

namespace ERM.Application.Interfaces.Services
{
    public interface IFabricColorService
    {
        Task<IReadOnlyList<FabricColorDto>> GetAllAsync(CancellationToken ct = default);
        Task<FabricColorDto> GetOrCreateAsync(string color, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
