using ERM.Core.Domain.Entities.Base;

namespace ERM.Core.Domain.Entities
{
    public class Ironer : Identity
    {
        public Guid EmployeeId { get; private set; }
        public Employee Employee { get; private set; } = null!;

        protected Ironer() { }

        public Ironer(Guid employeeId)
        {
            EmployeeId = employeeId;
        }
    }

}
