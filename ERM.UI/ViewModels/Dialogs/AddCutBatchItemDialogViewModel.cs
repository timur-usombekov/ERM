using ERM.Application.DTOs;
using ERM.UI.ViewModels.Base;

namespace ERM.UI.ViewModels.Dialogs
{
    public class AddCutBatchItemDialogViewModel : ViewModelBase
    {
        public IReadOnlyList<ClothingModelDto> AvailableModels { get; }

        private ClothingModelDto? _selectedModel;
        public ClothingModelDto? SelectedModel
        {
            get => _selectedModel;
            set => SetField(ref _selectedModel, value);
        }

        private string _color = string.Empty;
        public string Color
        {
            get => _color;
            set => SetField(ref _color, value);
        }

        private string _quantityText = string.Empty;
        public string QuantityText
        {
            get => _quantityText;
            set => SetField(ref _quantityText, value);
        }

        public int Quantity => int.TryParse(QuantityText, out var q) ? q : 0;

        public bool IsValid => SelectedModel is not null && !string.IsNullOrWhiteSpace(Color) && Quantity > 0;

        public AddCutBatchItemDialogViewModel(IReadOnlyList<ClothingModelDto> availableModels)
        {
            AvailableModels = availableModels;
        }
    }
}