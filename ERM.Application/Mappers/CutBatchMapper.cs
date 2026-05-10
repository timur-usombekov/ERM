using ERM.Application.DTOs;
using ERM.Core.Domain.Entities;

namespace ERM.Application.Mappers
{
    public static class CutBatchMapper
    {
        public static CutBatchDto ToDto(this CutBatch b) => new()
        {
            Id = b.Id,
            Title = b.Title,
            Date = b.Date,
            Items = b.Items.Select(i => new CutBatchItemDto
            {
                Id = i.Id,
                ClothingModelId = i.ClothingModelId,
                ClothingModelName = i.ClothingModel?.Name ?? "Неизвестно",
                Color = i.Color,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}