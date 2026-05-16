using ERM.Application.DTOs;
using ERM.Application.Interfaces.Services;
using ERM.UI.ViewModels.Base;
using System.Collections.ObjectModel;

namespace ERM.UI.ViewModels
{
    public class PayrollViewModel : ViewModelBase, INavigationAware
    {
        private readonly IWorkAssignmentService _workService;

        public ObservableCollection<SeamstressPayrollDto> PayrollList { get; } = [];

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
        public decimal GrandTotal => PayrollList.Sum(p => p.TotalSalary);

        public PayrollViewModel(IWorkAssignmentService workService)
        {
            _workService = workService;
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

                PayrollList.Clear();
                foreach (var item in payrolls)
                {
                    PayrollList.Add(item);
                }

                OnPropertyChanged(nameof(GrandTotal));
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}