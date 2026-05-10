using ERM.UI.ViewModels.Base;

namespace ERM.UI.ViewModels.Dialogs
{
    public class AddEmployeeDialogViewModel : ViewModelBase
    {
        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set  
            {   
                SetField(ref _fullName, value); 
                OnPropertyChanged(nameof(IsValid)); 
            }
            
        }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                SetField(ref _phoneNumber, value);
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private string? _notes;
        public string? Notes
        {
            get => _notes;
            set
            {
                SetField(ref _notes, value);
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private bool _isSeamstress;
        public bool IsSeamstress
        {
            get => _isSeamstress;
            set
            {
                SetField(ref _isSeamstress, value);
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private string _machineNumber = string.Empty;
        public string MachineNumber
        {
            get => _machineNumber;
            set
            {
                SetField(ref _machineNumber, value);
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(FullName) &&
            !string.IsNullOrWhiteSpace(PhoneNumber) &&
            (!IsSeamstress || !string.IsNullOrWhiteSpace(MachineNumber));
    }
}