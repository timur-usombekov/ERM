using ERM.Application.DTOs;
using ERM.Core.Domain.Entities;
using ERM.Core.Domain.Entities.Enum;

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
            AssignedDate = w.AssignedDate,

            OperationName = w.OperationType switch
            {
                OperationType.Sewing => "Пошив",
                OperationType.Ironing => "ВТО / Глажка",
                OperationType.Cutting => "Раскрой",
                _ => "Прочее"
            }

        };
    }
}