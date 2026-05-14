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
            DeclaredQuantity = b.DeclaredQuantity,
            UnallocatedQuantity = b.DeclaredQuantity - b.Items.Sum(i => i.Quantity),

            Items = b.Items.Select(i => i.ToDto()).ToList()

        };
    }
}