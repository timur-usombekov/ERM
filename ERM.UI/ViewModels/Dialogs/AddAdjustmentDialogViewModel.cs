using ERM.Application.DTOs;
using ERM.UI.ViewModels.Base;
using System;

namespace ERM.UI.ViewModels.Dialogs
{
    public class AddAdjustmentDialogViewModel : ViewModelBase
    {
        public string EmployeeName { get; }
        public Guid EmployeeId { get; }

        private string _amountText = string.Empty;
        public string AmountText
        {
            get => _amountText;
            set { SetField(ref _amountText, value); OnPropertyChanged(nameof(IsValid)); }
        }

        private string _reason = string.Empty;
        public string Reason
        {
            get => _reason;
            set { SetField(ref _reason, value); OnPropertyChanged(nameof(IsValid)); }
        }

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetField(ref _selectedDate, value);
        }

        public bool IsValid =>
            decimal.TryParse(AmountText, out var amount) && amount != 0 &&
            !string.IsNullOrWhiteSpace(Reason);

        public AddAdjustmentDialogViewModel(EmployeePayrollDto employee)
        {
            EmployeeName = employee.EmployeeName;
            EmployeeId = employee.EmployeeId;
        }
    }
}