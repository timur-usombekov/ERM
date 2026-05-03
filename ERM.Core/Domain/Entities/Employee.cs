using ERM.Core.Domain.Entities.Base;

namespace ERM.Core.Domain.Entities
{
    public class Employee : Identity
    {
        public string FullName { get; private set; } = null!;
        public string PhoneNumber { get; private set; } = null!;
        public string? Notes { get; private set; }

        public Seamstress? Seamstress { get; private set; }

        protected Employee() { }

        public Employee(string fullName, string phoneNumber)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
        }

        public void UpdateNotes(string? notes)
        {
            Notes = notes;
        }

        public void UpdateContacts(string fullName, string phoneNumber)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
        }
        public void AssignSeamstressRole(string machineNumber)
        {
            if (IsSeamstress)
                throw new InvalidOperationException("Сотрудник уже является швеёй.");

            Seamstress = new Seamstress(Id, machineNumber);
        }

        public void RevokeSeamstressRole()
        {
            if (!IsSeamstress)
                throw new InvalidOperationException("Сотрудник не является швеёй.");

            Seamstress = null;
        }

        public void UpdateMachineNumber(string machineNumber)
        {
            if (!IsSeamstress)
                throw new InvalidOperationException("Сотрудник не является швеёй.");

            Seamstress!.UpdateMachineNumber(machineNumber);
        }
        public bool IsSeamstress => Seamstress is not null;
    }
}