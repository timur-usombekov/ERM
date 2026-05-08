using ERM.Application.DTOs;
using ERM.Core.Domain.Entities;

namespace ERM.Application.Mappers
{
    public static class EmployeeMapper
    {
        public static EmployeeDto ToDto(this Employee employee)
        {
            if (employee is null) return null!; 

            return new EmployeeDto
            {
                Id = employee.Id,
                FullName = employee.FullName,
                PhoneNumber = employee.PhoneNumber,
                Notes = employee.Notes,

                IsSeamstress = employee.IsSeamstress,
                MachineNumber = employee.Seamstress?.MachineNumber
            };
        }
    }
}
