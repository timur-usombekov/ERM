using ERM.Application.DTOs;
using ERM.Application.Interfaces.Services;
using ERM.UI.ViewModels.Base;
using ERM.UI.ViewModels.Dialogs;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace ERM.UI.ViewModels
{
    public class ClothingModelsViewModel : ViewModelBase
    {
        private readonly IClothingModelService _service;

        public ObservableCollection<ClothingModelDto> Models { get; } = [];

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetField(ref _isLoading, value);
        }

        public AsyncRelayCommand LoadModelsCommand { get; }
        public AsyncRelayCommand AddModelCommand { get; }
        public AsyncRelayCommand<ClothingModelDto> EditModelCommand { get; }
        public AsyncRelayCommand<ClothingModelDto> DeleteModelCommand { get; }

        public ClothingModelsViewModel(IClothingModelService service)
        {
            _service = service;
            LoadModelsCommand = new AsyncRelayCommand(_ => LoadAsync());
            AddModelCommand = new AsyncRelayCommand(_ => AddAsync());
            EditModelCommand = new AsyncRelayCommand<ClothingModelDto>(EditAsync);
            DeleteModelCommand = new AsyncRelayCommand<ClothingModelDto>(DeleteAsync);

            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var models = await _service.GetAllAsync();
                Models.Clear();
                foreach (var m in models) Models.Add(m);
            }
            finally { IsLoading = false; }
        }

        private async Task AddAsync()
        {
            var vm = new AddClothingModelDialogViewModel();
            var result = await DialogHost.Show(vm, "RootDialog");

            if (result is not AddClothingModelDialogViewModel r || !r.IsValid) return;

            var (isSuccess, _) = await ExecuteSafeAsync(() => _service.CreateAsync(r.Name, r.Description));
            if (!isSuccess) return;

            await LoadAsync();
        }

        private async Task EditAsync(ClothingModelDto? dto)
        {
            if (dto is null) return;

            var vm = new EditClothingModelDialogViewModel(dto);
            var result = await DialogHost.Show(vm, "RootDialog");

            if (result is not EditClothingModelDialogViewModel r || !r.IsValid) return;

            var (isSuccess, _) = await ExecuteSafeAsync(() => _service.EditAsync(r.ModelId, r.Name, r.Description));
            if (!isSuccess) return;

            await LoadAsync();
        }

        private async Task DeleteAsync(ClothingModelDto? dto)
        {
            if (dto is null) return;

            var confirmed = await DialogHost.Show(
                new ConfirmDialogViewModel($"Удалить модель «{dto.Name}»?"), "RootDialog");

            if (confirmed?.ToString() != "True") return;

            if (!await ExecuteSafeAsync(() => _service.DeleteAsync(dto.Id))) return;

            Models.Remove(dto);
        }
    }
}
