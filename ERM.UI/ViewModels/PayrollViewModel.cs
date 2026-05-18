using ERM.Application.DTOs;
using ERM.Application.Interfaces.Services;
using ERM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using ERM.UI.ViewModels.Dialogs;
using MaterialDesignThemes.Wpf;

namespace ERM.UI.ViewModels
{
    public class PayrollViewModel : ViewModelBase, INavigationAware
    {
        private readonly IWorkAssignmentService _workService;

        public ObservableCollection<EmployeePayrollDto> SeamstressesPayroll { get; } = [];
        public ObservableCollection<EmployeePayrollDto> CuttersPayroll { get; } = [];

        // По умолчанию текущая неделю (с понедельника по воскресенье)
        private DateTime _startDate = GetStartOfWeek();
        public DateTime StartDate
        {
            get => _startDate;
            set 
            { 
                SetField(ref _startDate, value);
                _ = CalculatePayrollAsync();
            }
        }

        private DateTime _endDate = GetStartOfWeek().AddDays(6);
        public DateTime EndDate
        {
            get => _endDate;
            set 
            { 
                SetField(ref _endDate, value); 
                _ = CalculatePayrollAsync(); 
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetField(ref _isLoading, value);
        }

        // Вычисляемое свойство для общей суммы по всему цеху
        public decimal GrandTotal => SeamstressesPayroll.Sum(p => p.TotalToPay) + CuttersPayroll.Sum(p => p.TotalToPay);

        public AsyncRelayCommand<EmployeePayrollDto> AddAdjustmentCommand { get; }
        public AsyncRelayCommand<EmployeePayrollDto> PayCommand { get; }

        public PayrollViewModel(IWorkAssignmentService workService)
        {
            _workService = workService;
            AddAdjustmentCommand = new AsyncRelayCommand<EmployeePayrollDto>(AddAdjustmentAsync);
            PayCommand = new AsyncRelayCommand<EmployeePayrollDto>(PayAsync, dto => dto != null && dto.TotalToPay > 0);

        }

        public Task OnNavigatedToAsync() => CalculatePayrollAsync();
        private static DateTime GetStartOfWeek()
        {
            int diff = (7 + (DateTime.Today.DayOfWeek - DayOfWeek.Monday)) % 7;
            return DateTime.Today.AddDays(-diff);
        }

        private async Task CalculatePayrollAsync()
        {
            if (StartDate > EndDate)
            {
                // Небольшая защита от дурака
                var temp = StartDate;
                StartDate = EndDate;
                EndDate = temp;
            }

            IsLoading = true;
            try
            {
                var payrolls = await _workService.GetPayrollAsync(
                    DateOnly.FromDateTime(StartDate),
                    DateOnly.FromDateTime(EndDate));

                SeamstressesPayroll.Clear();
                CuttersPayroll.Clear();

                foreach (var item in payrolls)
                {
                    // Раскидываем по спискам
                    if (item.IsSeamstress) SeamstressesPayroll.Add(item);
                    if (item.IsCutter) CuttersPayroll.Add(item);
                }


                OnPropertyChanged(nameof(GrandTotal));
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddAdjustmentAsync(EmployeePayrollDto? employee)
        {
            if (employee is null) return;

            var vm = new AddAdjustmentDialogViewModel(employee);
            var result = await DialogHost.Show(vm, "RootDialog");

            if (result is not AddAdjustmentDialogViewModel r || !r.IsValid) return;

            decimal amount = decimal.Parse(r.AmountText);
            var date = DateOnly.FromDateTime(r.SelectedDate);

            var isSuccess = await ExecuteSafeAsync(() =>
                _workService.AddAdjustmentAsync(employee.EmployeeId, date, amount, r.Reason));

            if (isSuccess)
                _ = CalculatePayrollAsync();
        }

        private async Task PayAsync(EmployeePayrollDto? employee)
        {
            if (employee is null || employee.TotalToPay <= 0) return;

            var confirmed = await MaterialDesignThemes.Wpf.DialogHost.Show(
                new ConfirmDialogViewModel(
                    $"Провести выплату {employee.TotalToPay:0.##} ₴ сотруднику {employee.EmployeeName}?",
                    "Выплатить",
                    false),
                "RootDialog");


            if (confirmed?.ToString() != "True") return;

            var isSuccess = await ExecuteSafeAsync(() =>
                _workService.PaySalaryAsync(employee.EmployeeId, DateOnly.FromDateTime(DateTime.Today), employee.TotalToPay));

            if (isSuccess)
                _ = CalculatePayrollAsync();
        }

    }
}