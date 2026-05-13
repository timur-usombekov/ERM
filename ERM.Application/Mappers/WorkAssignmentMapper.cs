using ERM.Application.DTOs;
using ERM.Core.Domain.Entities;

namespace ERM.Application.Mappers
{
    public static class WorkAssignmentMapper
    {
        public static WorkAssignmentDto ToDto(this WorkAssignment w) => new()
        {
            Id = w.Id,
            SeamstressName = w.Seamstress?.Employee?.FullName ?? "Неизвестно",
            MachineNumber = w.Seamstress?.MachineNumber ?? "?",
            ClothingModelName = w.CutBatchItem?.ClothingModel?.Name ?? "Неизвестно",
            Color = w.CutBatchItem?.FabricColor?.Name ?? "-",
            Size = w.Size,
            Quantity = w.Quantity,
            AssignedDate = w.AssignedDate
        };
    }
}