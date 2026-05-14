using ERM.Application.DTOs;
using ERM.Core.Domain.Entities;

namespace ERM.Application.Mappers
{
    public static class CutBatchItemMapper
    {
        public static CutBatchItemDto ToDto(this CutBatchItem i) => new()
        {
            Id = i.Id,
            ClothingModelId = i.ClothingModelId,
            ClothingModelName = i.ClothingModel?.Name ?? "Неизвестно",
            Color = i.FabricColor?.Name ?? "Неизвестно",
            Quantity = i.Quantity,
            AvailableQuantity = i.Quantity - i.IssuedQuantity
        };
    }
}
