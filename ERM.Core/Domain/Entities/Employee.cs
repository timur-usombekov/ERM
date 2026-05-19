using ERM.Core.Domain.Entities.Base;

namespace ERM.Core.Domain.Entities
{
    public class Employee : Identity
    {
        public string FullName { get; private set; } = null!;
        public string PhoneNumber { get; private set; } = null!;
        public string? Notes { get; private set; }

        public Cutter? Cutter { get; private set; }
        public bool IsCutter => Cutter is not null;

        public Seamstress? Seamstress { get; private set; }
        public bool IsSeamstress => Seamstress is not null;

        public Ironer? Ironer { get; private set; }
        public bool IsIroner => Ironer is not null;

        protected Employee() { }

        public Employee(string fullName, string phoneNumber)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
        }

        public void UpdateGeneralInfo(string fullName, string phoneNumber, string? notes)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
            Notes = notes;
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

        public void AssignCutterRole(decimal percentage)
        {
            if (IsCutter) throw new InvalidOperationException("Сотрудник уже является закройщиком.");
            Cutter = new Cutter(Id, percentage);
        }

        public void RevokeCutterRole()
        {
            if (!IsCutter) throw new InvalidOperationException("Сотрудник не является закройщиком.");
            Cutter = null;
        }

        public void UpdateCutterPercentage(decimal percentage)
        {
            if (!IsCutter) throw new InvalidOperationException("Сотрудник не является закройщиком.");
            Cutter!.UpdatePercentage(percentage);
        }

        public void AssignIronerRole()
        {
            if (IsIroner)
                throw new InvalidOperationException("Сотрудник уже является гладильщицей.");

            Ironer = new Ironer(Id);
        }

        public void RevokeIronerRole()
        {
            if (!IsIroner)
                throw new InvalidOperationException("Сотрудник не является гладильщицей.");

            Ironer = null;
        }

    }
}