using ERM.Core.Domain.Entities.Enum;
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
        public ObservableCollection<EmployeeDto> Ironers { get; } = [];

        public IEnumerable<CutBatchItemDto> DisplayedCutItems =>
            IsSubstitutionMode ? _availableCutItemsToSubstitute : _availableCutItemsToSew;

        private ObservableCollection<CutBatchItemDto> _availableCutItemsToSew = [];
        private ObservableCollection<CutBatchItemDto> _availableCutItemsToSubstitute = [];

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

        private EmployeeDto? _selectedShiftIroner;
        public EmployeeDto? SelectedShiftIroner
        {
            get => _selectedShiftIroner;
            set 
            { 
                SetField(ref _selectedShiftIroner, value); 
                ClearError();
                _ = ReloadSubstitutionBalancesAsync();
            }
        }

        private CutBatchItemDto? _selectedCutItem;
        public CutBatchItemDto? SelectedCutItem
        {
            get => _selectedCutItem;
            set { SetField(ref _selectedCutItem, value); ClearError(); }
        }

        private bool _isSubstitutionMode;
        public bool IsSubstitutionMode
        {
            get => _isSubstitutionMode;
            set
            {
                SetField(ref _isSubstitutionMode, value);
                ClearError();
                QuantityText = string.Empty;
                SelectedCutItem = null; // Сбрасываем выбранный крой при смене режима
                OnPropertyChanged(nameof(DisplayedCutItems));

            }
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
                Guid? savedIronerId = SelectedShiftIroner?.Id;
                string? savedSize = SelectedSize;

                var assignments = await _workService.GetTodayAssignmentsAsync();
                TodayAssignments.Clear();
                foreach (var a in assignments) TodayAssignments.Add(a);

                var employees = await _employeeService.GetAllAsync();
                Seamstresses.Clear();
                Ironers.Clear();
                foreach (var s in employees.Where(e => e.IsSeamstress)) Seamstresses.Add(s);
                foreach (var i in employees.Where(e => e.IsIroner)) Ironers.Add(i);

                // Загружаем крой ДЛЯ ПОШИВА
                var batches = await _cutBatchService.GetAllAsync();
                _availableCutItemsToSew.Clear();
                var activeItems = batches.Where(b => !b.IsClosed).SelectMany(b => b.Items).Where(i => i.AvailableQuantity > 0);
                foreach (var item in activeItems) _availableCutItemsToSew.Add(item);

                if (savedIronerId.HasValue)
                    SelectedShiftIroner = Ironers.FirstOrDefault(i => i.Id == savedIronerId.Value);
                else if (Ironers.Any())
                    SelectedShiftIroner = Ironers.First();

                // Важно: балансы подмены обновятся автоматически через сеттер SelectedShiftIroner

                if (savedSeamstressId.HasValue) SelectedSeamstress = Seamstresses.FirstOrDefault(s => s.Id == savedSeamstressId.Value);
                SelectedSize = savedSize;

                // Восстанавливаем крой с учетом режима
                if (savedCutItemId.HasValue)
                    SelectedCutItem = DisplayedCutItems.FirstOrDefault(c => c.Id == savedCutItemId.Value);

                OnPropertyChanged(nameof(DisplayedCutItems));
            }
            finally { IsLoading = false; }
        }


        private async Task IssueWorkAsync()
        {
            if (SelectedSeamstress is null) { ErrorMessage = IsSubstitutionMode ? "Выберите сотрудника на подмене" : "Выберите швею"; return; }
            if (SelectedCutItem is null) { ErrorMessage = "Выберите крой"; return; }
            if (!IsSubstitutionMode && string.IsNullOrWhiteSpace(SelectedSize)) { ErrorMessage = "Укажите размер"; return; }
            if (!int.TryParse(QuantityText, out int qty) || qty <= 0) { ErrorMessage = "Некорректное количество"; return; }

            Guid savedSeamstressId = SelectedSeamstress.Id;
            Guid savedCutItemId = SelectedCutItem.Id;

            bool isSuccess = false;

            if (!IsSubstitutionMode)
            {
                // Обычная выдача пошива
                if (SelectedShiftIroner is null) { ErrorMessage = "Выберите гладильщицу на смене!"; return; }

                var result = await ExecuteSafeAsync(() =>
                    _workService.IssueSewingAsync(
                        SelectedSeamstress.Id,
                        SelectedShiftIroner.Id,
                        SelectedCutItem.Id,
                        SelectedSize!,
                        qty));

                isSuccess = result;
            }
            else
            {
                if (SelectedShiftIroner is null) { ErrorMessage = "Системе нужно знать основную гладильщицу для вычета!"; return; }

                var result = await ExecuteSafeAsync(() =>
                    // Этот метод создаст глажку для подменщицы и вычтет это кол-во у основной гладильщицы
                    _workService.RegisterIroningSubstitutionAsync(
                        SelectedSeamstress.Id,        // Кто фактически погладил (швея)
                        SelectedShiftIroner.Id,       // У кого вычитаем (основная гладильщица)
                        SelectedCutItem.Id,
                        qty));

                isSuccess = result;
            }

            if (isSuccess)
            {
                QuantityText = string.Empty;
                ErrorMessage = null;

                await LoadAsync();

                SelectedSeamstress = Seamstresses.FirstOrDefault(s => s.Id == savedSeamstressId);
                SelectedCutItem = _availableCutItemsToSew.FirstOrDefault(c => c.Id == savedCutItemId);
            }
        }


        private async Task DeleteAsync(WorkAssignmentDto? dto)
        {
            if (dto is null) return;

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
        private async Task ReloadSubstitutionBalancesAsync()
        {
            if (SelectedShiftIroner == null) return;

            var balances = await _workService.GetAvailableForSubstitutionAsync(SelectedShiftIroner.Id);

            _availableCutItemsToSubstitute.Clear();
            foreach (var b in balances) _availableCutItemsToSubstitute.Add(b);

            // Если мы в режиме подмены, уведомляем UI, что цифры обновились
            if (IsSubstitutionMode)
                OnPropertyChanged(nameof(DisplayedCutItems));
        }

        private void ClearError() => ErrorMessage = null;

    }
}