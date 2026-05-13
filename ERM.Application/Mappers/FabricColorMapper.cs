using ERM.Application.DTOs;
using ERM.Core.Domain.Entities;

namespace ERM.Application.Mappers
{
    public static class FabricColorMapper
    {
        public static FabricColorDto ToDto(this FabricColor c) => new()
        {
            Id = c.Id,
            Name = c.Name,
        };
    }
}
