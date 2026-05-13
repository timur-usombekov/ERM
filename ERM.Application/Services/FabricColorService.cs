using ERM.Application.DTOs;
using ERM.Application.Mappers;
using ERM.Application.Interfaces.Repositories;
using ERM.Application.Interfaces.Services;
using ERM.Core.Domain.Entities;

namespace ERM.Application.Services
{
    public class FabricColorService: IFabricColorService
    {
        private readonly IFabricColorRepository _repo;

        public FabricColorService(IFabricColorRepository repo) => _repo = repo;

        public async Task<IReadOnlyList<FabricColorDto>> GetAllAsync(CancellationToken ct = default)
        {
            var colors = await _repo.GetAllAsync(ct);
            return colors.Select(c => c.ToDto()).ToList();
        }

        public async Task<FabricColorDto> GetOrCreateAsync(string name, CancellationToken ct = default)
        {
            var cleanName = name.Trim();

            var allColors = await _repo.GetAllAsync(ct);
            var existing = allColors.FirstOrDefault(c => c.Name.ToLower() == cleanName.ToLower());

            if (existing is not null)
                return new FabricColorDto { Id = existing.Id, Name = existing.Name };

            var newColor = new FabricColor(cleanName);
            await _repo.AddAsync(newColor, ct);

            return new FabricColorDto { Id = newColor.Id, Name = newColor.Name };
        }


        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
            => await _repo.DeleteAsync(id, ct);
    }
}
