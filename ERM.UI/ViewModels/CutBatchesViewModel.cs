using ERM.Application.DTOs;
using ERM.Application.Interfaces.Services;
using ERM.UI.ViewModelDTOs;
using ERM.UI.ViewModels.Base;
using ERM.UI.ViewModels.Dialogs;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace ERM.UI.ViewModels
{
    public class CutBatchesViewModel : ViewModelBase, INavigationAware
    {
        private readonly ICutBatchService _cutBatchService;
        private readonly IClothingModelService _clothingModelService;
        private readonly IFabricColorService _fabricColorService;
        private readonly IEmployeeService _employeeService;

        public ObservableCollection<CutBatchDto> Batches { get; } = [];

        public ObservableCollection<CutBatchModelGroup> GroupedItems { get; } = [];

        private CutBatchDto? _selectedBatch;
        public CutBatchDto? SelectedBatch
        {
            get => _selectedBatch;
            set
            {
                SetField(ref _selectedBatch, value);
                UpdateGroups();
                AddItemCommand.RaiseCanExecuteChanged();
            }
        }

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

        public AsyncRelayCommand LoadCommand { get; }
        public AsyncRelayCommand AddBatchCommand { get; }
        public AsyncRelayCommand AddItemCommand { get; }
        public AsyncRelayCommand<CutBatchDto> DeleteBatchCommand { get; }
        public AsyncRelayCommand<CutBatchDto> ToggleBatchStatusCommand { get; }

        public AsyncRelayCommand<CutBatchItemDto> EditItemCommand { get; }
        public AsyncRelayCommand<CutBatchItemDto> DeleteItemCommand { get; }

        public CutBatchesViewModel(
            ICutBatchService cutBatchService,
            IClothingModelService clothingModelService,
            IFabricColorService fabricColorService,
            IEmployeeService employeeService)
        {
            _cutBatchService = cutBatchService;
            _clothingModelService = clothingModelService;
            _fabricColorService = fabricColorService;
            _employeeService = employeeService;

            LoadCommand = new AsyncRelayCommand(_ => LoadAsync());
            AddBatchCommand = new AsyncRelayCommand(_ => AddBatchAsync());
            AddItemCommand = new AsyncRelayCommand(_ => AddItemAsync(), _ => SelectedBatch is not null);
            DeleteBatchCommand = new AsyncRelayCommand<CutBatchDto>(DeleteBatchAsync);
            ToggleBatchStatusCommand = new AsyncRelayCommand<CutBatchDto>(ToggleStatusAsync);
            EditItemCommand = new AsyncRelayCommand<CutBatchItemDto>(EditItemAsync);
            DeleteItemCommand = new AsyncRelayCommand<CutBatchItemDto>(DeleteItemAsync);

        }

        public Task OnNavigatedToAsync() => LoadAsync();

        private async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                Guid? savedId = SelectedBatch?.Id;

                var batches = await _cutBatchService.GetAllAsync();
                Batches.Clear();
                foreach (var b in batches) Batches.Add(b);

                SelectedBatch = savedId.HasValue
                    ? Batches.FirstOrDefault(b => b.Id == savedId.Value)
                    : Batches.FirstOrDefault();
            }
            finally { IsLoading = false; }
        }

        private void UpdateGroups()
        {
            GroupedItems.Clear();
            if (SelectedBatch is null) return;

            var groups = SelectedBatch.Items
                .GroupBy(i => i.ClothingModelName)
                .Select(g => new CutBatchModelGroup
                {
                    ModelName = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    CutBatchItems = new ObservableCollection<CutBatchItemDto>(g)
                });

            foreach (var group in groups)
            {
                GroupedItems.Add(group);
            }
        }

        private async Task AddBatchAsync()
        {
            var employees = await _employeeService.GetAllAsync();
            var dialogVm = new AddCutBatchDialogViewModel(employees);
            var result = await DialogHost.Show(dialogVm, "RootDialog");

            if (result is not AddCutBatchDialogViewModel vm || !vm.IsValid) return;

            var dateOnly = DateOnly.FromDateTime(vm.Date);

            var (isSuccess, newBatch) = await ExecuteSafeAsync(() =>
                _cutBatchService.CreateAsync(vm.Title, dateOnly, vm.DeclaredQuantity, vm.SelectedCutter!.Id));

            if (!isSuccess || newBatch is null) return;

            Batches.Insert(0, newBatch);
            SelectedBatch = newBatch;
        }


        private async Task DeleteBatchAsync(CutBatchDto? batch)
        {
            if (batch is null) return;

            var confirmed = await DialogHost.Show(new ConfirmDialogViewModel($"Удалить документ «{batch.Title}»?"), "RootDialog");
            if (confirmed?.ToString() != "True") return;

            if (!await ExecuteSafeAsync(() => _cutBatchService.DeleteAsync(batch.Id))) return;
            Batches.Remove(batch);
            SelectedBatch = Batches.FirstOrDefault();
        }

        private async Task AddItemAsync()
        {
            if (SelectedBatch is null) return;

            var models = await _clothingModelService.GetAllAsync();
            if (!models.Any())
            {
                // по хорошему нужно будет заменить на окно либо баннер с предложением создать модель
                //await ExecuteSafeAsync(() => throw new InvalidOperationException("Нет ни одной модели одежды. Создайте модель перед разделением кроя."));
                // Нужно будет добавить наблюдатель за ClothingModels и сбрасывать ErrorMessage при появлении моделей, но для простоты пока так
                ErrorMessage = "Нет ни одной модели одежды.";
                return;
            }
            ErrorMessage = null; // что бы не висела постоянно

            var colors = await _fabricColorService.GetAllAsync();

            var vm = new AddCutBatchItemDialogViewModel(models, colors);
            var result = await DialogHost.Show(vm, "RootDialog");

            if (result is not AddCutBatchItemDialogViewModel r || !r.IsValid) return;

            var (isColorSuccess, colorDto) = await ExecuteSafeAsync(() => _fabricColorService.GetOrCreateAsync(r.ColorText));
            if (!isColorSuccess || colorDto is null) return;

            var (isSuccess, newItem) = await ExecuteSafeAsync(() =>
                _cutBatchService.AddItemToBatchAsync(SelectedBatch.Id, r.SelectedModel!.Id, colorDto.Id, r.Quantity)
            );

            if (!isSuccess || newItem is null) return;
             await LoadAsync(); // LoadAsync сам вызовет UpdateGroups через Setter
        }
        private async Task ToggleStatusAsync(CutBatchDto? batch)
        {
            if (batch is null) return;
            await ExecuteSafeAsync(() => _cutBatchService.ToggleStatusAsync(batch.Id));
            await LoadAsync();
        }

        private async Task EditItemAsync(CutBatchItemDto? item)
        {
            if (item is null || SelectedBatch is null) return;

            var models = await _clothingModelService.GetAllAsync();
            var colors = await _fabricColorService.GetAllAsync();

            // Создаем диалог редактирования (по сути копия диалога создания, но с предзаполненными данными)
            var vm = new EditCutBatchItemDialogViewModel(item, models, colors);
            var result = await DialogHost.Show(vm, "RootDialog");

            if (result is not EditCutBatchItemDialogViewModel r || !r.IsValid) return;

            var (isColorSuccess, colorDto) = await ExecuteSafeAsync(() => _fabricColorService.GetOrCreateAsync(r.ColorText));
            if (!isColorSuccess || colorDto is null) return;

            var isSuccess = await ExecuteSafeAsync(() =>
                _cutBatchService.EditItemInBatchAsync(item.Id, r.SelectedModel!.Id, colorDto.Id, r.Quantity)
            );

            if (isSuccess) await LoadAsync();
        }

        private async Task DeleteItemAsync(CutBatchItemDto? item)
        {
            if (item is null) return;

            if (item.AvailableQuantity < item.Quantity)
            {
                ErrorMessage = "Нельзя удалить позицию, часть которой уже выдана швеям. Отредактируйте количество.";
                return;
            }

            var confirmed = await DialogHost.Show(new ConfirmDialogViewModel($"Удалить {item.ClothingModelName} ({item.Color})?"), "RootDialog");
            if (confirmed?.ToString() != "True") return;

            var isSuccess = await ExecuteSafeAsync(() => _cutBatchService.RemoveItemFromBatchAsync(item.Id));

            if (isSuccess) await LoadAsync();
        }

    }
}