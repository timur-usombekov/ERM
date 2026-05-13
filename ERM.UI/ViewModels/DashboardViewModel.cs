using ERM.Application.DTOs;
using ERM.Application.Interfaces.Services;
using ERM.UI.ViewModels.Base;
using ERM.UI.ViewModels.Dialogs;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace ERM.UI.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly IWorkAssignmentService _workService;
        private readonly IEmployeeService _employeeService;
        private readonly ICutBatchService _cutBatchService;

        public ObservableCollection<WorkAssignmentDto> TodayAssignments { get; } = [];

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetField(ref _isLoading, value);
        }

        public AsyncRelayCommand LoadCommand { get; }
        public AsyncRelayCommand IssueWorkCommand { get; }
        public AsyncRelayCommand<WorkAssignmentDto> DeleteAssignmentCommand { get; }

        public DashboardViewModel(
            IWorkAssignmentService workService,
            IEmployeeService employeeService,
            ICutBatchService cutBatchService)
        {
            _workService = workService;
            _employeeService = employeeService;
            _cutBatchService = cutBatchService;

            LoadCommand = new AsyncRelayCommand(_ => LoadAsync());
            IssueWorkCommand = new AsyncRelayCommand(_ => IssueWorkAsync());
            DeleteAssignmentCommand = new AsyncRelayCommand<WorkAssignmentDto>(DeleteAsync);

            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var assignments = await _workService.GetTodayAssignmentsAsync();
                TodayAssignments.Clear();
                foreach (var a in assignments) TodayAssignments.Add(a);
            }
            finally { IsLoading = false; }
        }
        private async Task IssueWorkAsync()
        {
            var allEmployees = await _employeeService.GetAllAsync();
            var seamstresses = allEmployees.Where(e => e.IsSeamstress).ToList();

            var batches = await _cutBatchService.GetAllAsync();

            var availableItems = batches
                .SelectMany(b => b.Items)
                .Where(i => i.AvailableQuantity > 0)
                .ToList();

            if (!seamstresses.Any())
            {
                await DialogHost.Show(new ErrorDialogViewModel("В базе нет ни одной швеи."), "RootDialog");
                return;
            }
            if (!availableItems.Any())
            {
                await DialogHost.Show(new ErrorDialogViewModel("Нет доступного кроя для выдачи."), "RootDialog");
                return;
            }

            var dialogVm = new AddWorkAssignmentDialogViewModel(seamstresses, availableItems);
            var result = await DialogHost.Show(dialogVm, "RootDialog");

            if (result is not AddWorkAssignmentDialogViewModel vm || !vm.IsValid) return;

            var (isSuccess, _) = await ExecuteSafeAsync(() =>
                _workService.IssueWorkAsync(
                    vm.SelectedSeamstress!.SeamstressId!.Value,
                    vm.SelectedCutBatchItem!.Id,
                    vm.SelectedSize!,
                    vm.Quantity
                )
            );

            if (isSuccess)
                await LoadAsync();
        }

        private async Task DeleteAsync(WorkAssignmentDto? dto)
        {
            if (dto is null) return;

            var confirmed = await DialogHost.Show(new ConfirmDialogViewModel("Удалить запись о выдаче?"), "RootDialog");
            if (confirmed?.ToString() != "True") return;

            if(!await ExecuteSafeAsync(() => _workService.DeleteAsync(dto.Id))) return;
            TodayAssignments.Remove(dto);
        }

    }
}