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
        public AsyncRelayCommand DeleteBatchCommand { get; }
        public AsyncRelayCommand AddItemCommand { get; }

        public CutBatchesViewModel(ICutBatchService cutBatchService, IClothingModelService clothingModelService)
        {
            _cutBatchService = cutBatchService;
            _clothingModelService = clothingModelService;

            LoadCommand = new AsyncRelayCommand(_ => LoadAsync());
            AddBatchCommand = new AsyncRelayCommand(_ => AddBatchAsync());
            DeleteBatchCommand = new AsyncRelayCommand(DeleteBatchAsync, _ => SelectedBatch is not null);
            AddItemCommand = new AsyncRelayCommand(AddItemAsync, _ => SelectedBatch is not null);

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
            // Позже можно вынести в диалоговое окно
            var newBatch = await _cutBatchService.CreateAsync($"Крой от {DateTime.Now:dd.MM.yyyy HH:mm}", DateOnly.FromDateTime(DateTime.Today));
            Batches.Insert(0, newBatch);
            SelectedBatch = newBatch;
        }

        private async Task DeleteBatchAsync(object? parameter)
        {
            if (SelectedBatch is null) return;

            var confirmed = await DialogHost.Show(new ConfirmDialogViewModel($"Удалить {SelectedBatch.Title}?"), "RootDialog");
            if (confirmed?.ToString() != "True") return;

            await _cutBatchService.DeleteAsync(SelectedBatch.Id);
            Batches.Remove(SelectedBatch);
            SelectedBatch = Batches.FirstOrDefault();
        }

        private async Task AddItemAsync(object? parameter)
        {
            if (SelectedBatch is null) return;

            var models = await _clothingModelService.GetAllAsync();
            if (!models.Any())
            {
                // Тут в идеале показать сообщение "Сначала создайте модели одежды"
                return;
            }

            var vm = new AddCutBatchItemDialogViewModel(models);
            var result = await DialogHost.Show(vm, "RootDialog");

            if (result is not AddCutBatchItemDialogViewModel r || !r.IsValid) return;

            await _cutBatchService.AddItemToBatchAsync(SelectedBatch.Id, r.SelectedModel!.Id, r.Color, r.Quantity);

            await LoadAsync();

            // Восстанавливаем выбор (т.к. LoadAsync сбросит его)
            SelectedBatch = Batches.FirstOrDefault(b => b.Id == SelectedBatch.Id);
        }
    }
}