using ERM.Application.DTOs;
using ERM.UI.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ERM.UI.ViewModels.Dialogs
{
    public class EditCutBatchItemDialogViewModel : ViewModelBase
    {
        public IReadOnlyList<ClothingModelDto> AvailableModels { get; }
        public ObservableCollection<FabricColorDto> AvailableColors { get; }

        private ClothingModelDto? _selectedModel;
        public ClothingModelDto? SelectedModel
        {
            get => _selectedModel;
            set { SetField(ref _selectedModel, value); OnPropertyChanged(nameof(IsValid)); }
        }

        private string _colorText = string.Empty;
        public string ColorText
        {
            get => _colorText;
            set { SetField(ref _colorText, value); OnPropertyChanged(nameof(IsValid)); }
        }

        private string _quantityText = string.Empty;
        public string QuantityText
        {
            get => _quantityText;
            set { SetField(ref _quantityText, value); OnPropertyChanged(nameof(Quantity)); OnPropertyChanged(nameof(IsValid)); }
        }

        public int Quantity => int.TryParse(QuantityText, out var q) ? q : 0;

        public bool IsValid =>
            SelectedModel is not null &&
            Quantity > 0 &&
            !string.IsNullOrWhiteSpace(ColorText);

        public EditCutBatchItemDialogViewModel(
            CutBatchItemDto existingItem,
            IReadOnlyList<ClothingModelDto> availableModels,
            IEnumerable<FabricColorDto> colors)
        {
            AvailableModels = availableModels;
            AvailableColors = new ObservableCollection<FabricColorDto>(colors);

            // Предзаполняем данные существующей позиции
            SelectedModel = AvailableModels.FirstOrDefault(m => m.Id == existingItem.ClothingModelId);
            ColorText = existingItem.Color;
            QuantityText = existingItem.Quantity.ToString();
        }
    }
}