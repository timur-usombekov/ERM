using ERM.Application.DTOs;
using ERM.UI.ViewModels.Base;

namespace ERM.UI.ViewModels.Dialogs
{
    public class EditEmployeeDialogViewModel : ViewModelBase
    {
        public Guid EmployeeId { get; }

        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set => SetField(ref _fullName, value);
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetField(ref _phoneNumber, value);
        }

        private string? _notes;
        public string? Notes
        {
            get => _notes;
            set => SetField(ref _notes, value);
        }

        private bool _isSeamstress;
        public bool IsSeamstress
        {
            get => _isSeamstress;
            set => SetField(ref _isSeamstress, value);
        }

        private string _machineNumber;
        public string MachineNumber
        {
            get => _machineNumber;
            set => SetField(ref _machineNumber, value);
        }

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(FullName) &&
            !string.IsNullOrWhiteSpace(PhoneNumber) &&
            (!IsSeamstress || !string.IsNullOrWhiteSpace(MachineNumber));

        // Принимаем существующего сотрудника и заполняем поля его данными
        public EditEmployeeDialogViewModel(EmployeeDto employee)
        {
            EmployeeId = employee.Id;
            _fullName = employee.FullName;
            _phoneNumber = employee.PhoneNumber;
            _notes = employee.Notes;
            _isSeamstress = employee.IsSeamstress;
            _machineNumber = employee.MachineNumber ?? string.Empty;
        }
    }
}