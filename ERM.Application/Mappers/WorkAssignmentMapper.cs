using ERM.Application.DTOs;
using ERM.Core.Domain.Entities;

namespace ERM.Application.Mappers
{
    public static class WorkAssignmentMapper
    {
        public static WorkAssignmentDto ToDto(this WorkAssignment w) => new()
        {
            Id = w.Id,
            EmployeeName = w.Employee?.FullName ?? "Неизвестно",
            MachineNumber = w.Employee?.Seamstress?.MachineNumber ?? "?",
            ClothingModelName = w.CutBatchItem?.ClothingModel?.Name ?? "-",
            Color = w.CutBatchItem?.FabricColor?.Name ?? "-",
            Size = w.Size ?? "-", 
            Quantity = w.Quantity,
            AssignedDate = w.AssignedDate
        };
    }
}