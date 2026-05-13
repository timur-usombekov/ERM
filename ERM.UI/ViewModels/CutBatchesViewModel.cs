using ERM.Application.DTOs;
using ERM.Application.Interfaces.Services;
using ERM.UI.ViewModels.Base;
using ERM.UI.ViewModels.Dialogs;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace ERM.UI.ViewModels
{
    public class CutBatchesViewModel : ViewModelBase
    {
        private readonly ICutBatchService _cutBatchService;
        private readonly IClothingModelService _clothingModelService;
        private readonly IFabricColorService _fabricColorService;

        public ObservableCollection<CutBatchDto> Batches { get; } = [];

        private CutBatchDto? _selectedBatch;
        public CutBatchDto? SelectedBatch
        {
            get => _selectedBatch;
            set => SetField(ref _selectedBatch, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetField(ref _isLoading, value);
        }

        public AsyncRelayCommand LoadCommand { get; }
        public AsyncRelayCommand AddBatchCommand { get; }
        public AsyncRelayCommand AddItemCommand { get; }

        public AsyncRelayCommand<CutBatchDto> DeleteBatchCommand { get; }

        public CutBatchesViewModel(
            ICutBatchService cutBatchService, 
            IClothingModelService clothingModelService,
            IFabricColorService fabricColorService)
        {
            _cutBatchService = cutBatchService;
            _clothingModelService = clothingModelService;
            _fabricColorService = fabricColorService;

            LoadCommand = new AsyncRelayCommand(_ => LoadAsync());
            AddBatchCommand = new AsyncRelayCommand(_ => AddBatchAsync());

            AddItemCommand = new AsyncRelayCommand(_ => AddItemAsync(), _ => SelectedBatch is not null);

            DeleteBatchCommand = new AsyncRelayCommand<CutBatchDto>(DeleteBatchAsync);

            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var batches = await _cutBatchService.GetAllAsync();
                Batches.Clear();
                foreach (var b in batches) Batches.Add(b);

                SelectedBatch = Batches.FirstOrDefault();
            }
            finally { IsLoading = false; }
        }

        private async Task AddBatchAsync()
        {
            var dialogVm = new AddCutBatchDialogViewModel();
            var result = await DialogHost.Show(dialogVm, "RootDialog");

            if (result is not AddCutBatchDialogViewModel vm || !vm.IsValid) return;

            var dateOnly = DateOnly.FromDateTime(vm.Date);
            var (isSuccess, newBatch) = await ExecuteSafeAsync(() => _cutBatchService.CreateAsync(vm.Title, dateOnly, vm.DeclaredQuantity));
            if (!isSuccess || newBatch is null) return;

            Batches.Insert(0, newBatch);
            SelectedBatch = newBatch;
        }

        private async Task DeleteBatchAsync(CutBatchDto? batch)
        {
            if (batch is null) return;

            var confirmed = await DialogHost.Show(new ConfirmDialogViewModel($"Удалить документ «{batch.Title}»?"), "RootDialog");
            if (confirmed?.ToString() != "True") return;

            if(!await ExecuteSafeAsync(() => _cutBatchService.DeleteAsync(batch.Id))) return;
            Batches.Remove(batch);
            SelectedBatch = Batches.FirstOrDefault();
        }

        private async Task AddItemAsync()
        {
            if (SelectedBatch is null) return;

            var models = await _clothingModelService.GetAllAsync();
            if (!models.Any()) return;

            var colors = await _fabricColorService.GetAllAsync();

            var vm = new AddCutBatchItemDialogViewModel(models, colors);
            var result = await DialogHost.Show(vm, "RootDialog");

            if (result is not AddCutBatchItemDialogViewModel r || !r.IsValid) return;

            var (isColorSuccess, colorDto) = await ExecuteSafeAsync(() => _fabricColorService.GetOrCreateAsync(r.ColorText));
            if (!isColorSuccess || colorDto is null) return;

            if (!await ExecuteSafeAsync(() =>
                _cutBatchService.AddItemToBatchAsync(SelectedBatch.Id, r.SelectedModel!.Id, colorDto.Id, r.Quantity)
            )) return;

            await LoadAsync();
            SelectedBatch = Batches.FirstOrDefault(b => b.Id == SelectedBatch.Id);
        }
    }
}