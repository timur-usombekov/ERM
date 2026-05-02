using ERM.Core.Domain.Entities.Base;

namespace ERM.Core.Domain.Entities
{
    public class Seamstress : Identity
    {
        public Guid EmployeeId { get; private set; }
        public string MachineNumber { get; private set; } = null!;

        public Employee Employee { get; private set; } = null!;

        protected Seamstress() { }

        public Seamstress(Guid employeeId, string machineNumber)
        {
            EmployeeId = employeeId;
            MachineNumber = machineNumber;
        }

        public void UpdateMachineNumber(string machineNumber)
        {
            MachineNumber = machineNumber;
        }
    }
}