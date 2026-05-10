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

            // Тут надо будет потом отфильтровать только те партии, которые еще не сшиты до конца. Но пока берем все актуальные.
            var batches = await _cutBatchService.GetAllAsync();
            var availableItems = batches.SelectMany(b => b.Items).ToList();

            if (!seamstresses.Any() || !availableItems.Any())
            {
                // Не забыть показать потом, что нет швей или нет кроя
                return;
            }

            var dialogVm = new AddWorkAssignmentDialogViewModel(seamstresses, availableItems);
            var result = await DialogHost.Show(dialogVm, "RootDialog");

            if (result is not AddWorkAssignmentDialogViewModel vm || !vm.IsValid) return;

            await _workService.IssueWorkAsync(
                vm.SelectedSeamstress!.SeamstressId!.Value,
                vm.SelectedCutBatchItem!.Id,
                vm.Size,
                vm.Quantity
            );

            await LoadAsync();
        }

        private async Task DeleteAsync(WorkAssignmentDto? dto)
        {
            if (dto is null) return;

            var confirmed = await DialogHost.Show(new ConfirmDialogViewModel("Удалить запись о выдаче?"), "RootDialog");
            if (confirmed?.ToString() != "True") return;

            await _workService.DeleteAsync(dto.Id);
            TodayAssignments.Remove(dto);
        }
    }
}