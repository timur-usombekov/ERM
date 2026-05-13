using ERM.Application.DTOs;
using ERM.Application.Interfaces.Services;
using ERM.UI.ViewModels.Base;
using System.Collections.ObjectModel;

namespace ERM.UI.ViewModels
{
    public class DashboardViewModel : ViewModelBase, INavigationAware
    {
        private readonly IWorkAssignmentService _workService;
        private readonly IEmployeeService _employeeService;
        private readonly ICutBatchService _cutBatchService;

        #region Списки данных
        public ObservableCollection<WorkAssignmentDto> TodayAssignments { get; } = [];
        public ObservableCollection<EmployeeDto> Seamstresses { get; } = [];
        public ObservableCollection<CutBatchItemDto> AvailableCutItems { get; } = [];

        public IReadOnlyList<string> AvailableSizes { get; } =
            [ "42", "44", "46", "48", "50", "52", "54", "56", "58", "60",
              "XS", "S", "M", "L", "XL", "2XL", "3XL", "4XL", "5XL", "Универсальный" ];
        #endregion

        #region Поля быстрой выдачи (Липкие)
        private EmployeeDto? _selectedSeamstress;
        public EmployeeDto? SelectedSeamstress
        {
            get => _selectedSeamstress;
            set { SetField(ref _selectedSeamstress, value); ClearError(); }
        }

        private CutBatchItemDto? _selectedCutItem;
        public CutBatchItemDto? SelectedCutItem
        {
            get => _selectedCutItem;
            set { SetField(ref _selectedCutItem, value); ClearError(); }
        }

        private string? _selectedSize;
        public string? SelectedSize
        {
            get => _selectedSize;
            set { SetField(ref _selectedSize, value); ClearError(); }
        }

        private string _quantityText = string.Empty;
        public string QuantityText
        {
            get => _quantityText;
            set { SetField(ref _quantityText, value); ClearError(); }
        }
        #endregion

        #region Состояния UI
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetField(ref _isLoading, value);
        }

        private string? _errorMessage;
        public string? ErrorMessage
        {
            get => _errorMessage;
            private set => SetField(ref _errorMessage, value);
        }
        #endregion

        #region Команды
        public AsyncRelayCommand LoadCommand { get; }
        public AsyncRelayCommand IssueWorkCommand { get; }
        public AsyncRelayCommand<WorkAssignmentDto> DeleteAssignmentCommand { get; }
        #endregion

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

        public async Task OnNavigatedToAsync()
        {
            await LoadAsync();
        }
        private async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                Guid? savedSeamstressId = SelectedSeamstress?.Id;
                Guid? savedCutItemId = SelectedCutItem?.Id;
                string? savedSize = SelectedSize;

                var assignments = await _workService.GetTodayAssignmentsAsync();
                TodayAssignments.Clear();
                foreach (var a in assignments) TodayAssignments.Add(a);

                var employees = await _employeeService.GetAllAsync();
                Seamstresses.Clear();
                foreach (var s in employees.Where(e => e.IsSeamstress)) Seamstresses.Add(s);

                var batches = await _cutBatchService.GetAllAsync();
                AvailableCutItems.Clear();
                foreach (var item in batches.SelectMany(b => b.Items).Where(i => i.AvailableQuantity > 0))
                {
                    AvailableCutItems.Add(item);
                }

                // возврат выбранных элементов
                if (savedSeamstressId.HasValue)
                {
                    SelectedSeamstress = Seamstresses.FirstOrDefault(s => s.Id == savedSeamstressId.Value);
                }
                if (savedCutItemId.HasValue)
                {
                    SelectedCutItem = AvailableCutItems.FirstOrDefault(c => c.Id == savedCutItemId.Value);
                }
                SelectedSize = savedSize;
            }
            finally { IsLoading = false; }
        }


        private async Task IssueWorkAsync()
        {
            if (SelectedSeamstress is null) { ErrorMessage = "Выберите швею"; return; }
            if (SelectedCutItem is null) { ErrorMessage = "Выберите крой"; return; }
            if (string.IsNullOrWhiteSpace(SelectedSize)) { ErrorMessage = "Укажите размер"; return; }
            if (!int.TryParse(QuantityText, out int qty) || qty <= 0) { ErrorMessage = "Некорректное количество"; return; }

            Guid savedSeamstressId = SelectedSeamstress.Id;
            Guid savedCutItemId = SelectedCutItem.Id;

            var (isSuccess, newAssignment) = await ExecuteSafeAsync(() =>
                _workService.IssueWorkAsync(SelectedSeamstress.SeamstressId!.Value, SelectedCutItem.Id, SelectedSize, qty));

            if (isSuccess && newAssignment != null)
            {
                TodayAssignments.Insert(0, newAssignment);
                QuantityText = string.Empty;
                ErrorMessage = null;

                await LoadAsync();

                SelectedSeamstress = Seamstresses.FirstOrDefault(s => s.Id == savedSeamstressId);
                SelectedCutItem = AvailableCutItems.FirstOrDefault(c => c.Id == savedCutItemId);
            }
        }


        private async Task DeleteAsync(WorkAssignmentDto? dto)
        {
            if (dto is null) return;

            // Используем ExecuteSafeAsync, чтобы поймать возможные ошибки базы
            var (isSuccess, _) = await ExecuteSafeAsync(async () =>
            {
                await _workService.DeleteAsync(dto.Id);
                return true;
            });

            if (isSuccess)
            {
                TodayAssignments.Remove(dto);
                _ = LoadAsync(); // Фоново обновляем остатки кроя
            }
        }

        private void ClearError() => ErrorMessage = null;

    }
}