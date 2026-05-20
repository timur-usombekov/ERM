using ERM.Application.DTOs;
using ERM.Core.Domain.Entities;

namespace ERM.Application.Mappers
{
    public static class ClothingModelMapper
    {
        public static ClothingModelDto ToDto(this ClothingModel m) => new()
        {
            Id = m.Id,
            Name = m.Name,
            SewingPrice = m.SewingPrice,
            IroningPrice = m.IroningPrice,
            Description = m.Description
        };
    }
}
