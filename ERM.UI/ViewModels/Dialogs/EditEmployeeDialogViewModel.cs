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

        private bool _isCutter;
        public bool IsCutter
        {
            get => _isCutter;
            set { SetField(ref _isCutter, value); OnPropertyChanged(nameof(IsValid)); }
        }

        private string _cutterPercentageText = string.Empty;
        public string CutterPercentageText
        {
            get => _cutterPercentageText;
            set { SetField(ref _cutterPercentageText, value); OnPropertyChanged(nameof(IsValid)); }
        }

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(FullName) &&
            !string.IsNullOrWhiteSpace(PhoneNumber) &&
            (!IsSeamstress || !string.IsNullOrWhiteSpace(MachineNumber)) &&
            (!IsCutter || (decimal.TryParse(CutterPercentageText, out var p) && p >= 0 && p <= 100));

        // Принимаем существующего сотрудника и заполняем поля его данными
        public EditEmployeeDialogViewModel(EmployeeDto employee)
        {
            EmployeeId = employee.Id;
            _fullName = employee.FullName;
            _phoneNumber = employee.PhoneNumber;
            _notes = employee.Notes;
            _isSeamstress = employee.IsSeamstress;
            _machineNumber = employee.MachineNumber ?? string.Empty;
            _isCutter = employee.IsCutter;
            _cutterPercentageText = employee.CutterPercentage?.ToString() ?? string.Empty;
        }
    }
}