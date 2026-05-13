using ERM.Application.DTOs;
using ERM.UI.ViewModels.Base;
using System.Collections.ObjectModel;

namespace ERM.UI.ViewModels.Dialogs
{
    public class AddWorkAssignmentDialogViewModel : ViewModelBase
    {
        public ObservableCollection<EmployeeDto> Seamstresses { get; }
        public ObservableCollection<CutBatchItemDto> CutBatchItems { get; }
        public IReadOnlyList<string> AvailableSizes { get; } =
            [ "42", "44", "46", "48", "50", "52", "54", "56", "58", "60",
              "XS", "S", "M", "L", "XL", "2XL", "3XL", "4XL", "5XL", "Универсальный" ];

        private EmployeeDto? _selectedSeamstress;

        public EmployeeDto? SelectedSeamstress
        {
            get => _selectedSeamstress;
            set
            {
                SetField(ref _selectedSeamstress, value);
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private CutBatchItemDto? _selectedCutBatchItem;
        public CutBatchItemDto? SelectedCutBatchItem
        {
            get => _selectedCutBatchItem;
            set
            {
                SetField(ref _selectedCutBatchItem, value);
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private string? _selectedSize;
        public string? SelectedSize
        {
            get => _selectedSize;
            set
            {
                SetField(ref _selectedSize, value);
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private string _quantityText = string.Empty;
        public string QuantityText
        {
            get => _quantityText;
            set
            {
                SetField(ref _quantityText, value);
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public int Quantity => int.TryParse(QuantityText, out var q) ? q : 0;



        public bool IsValid =>
            SelectedSeamstress is not null &&
            SelectedCutBatchItem is not null &&
            !string.IsNullOrWhiteSpace(SelectedSize) &&
            Quantity > 0;


        public AddWorkAssignmentDialogViewModel(IEnumerable<EmployeeDto> seamstresses, IEnumerable<CutBatchItemDto> cutBatchItems)
        {
            Seamstresses = new ObservableCollection<EmployeeDto>(seamstresses);
            CutBatchItems = new ObservableCollection<CutBatchItemDto>(cutBatchItems);
        }
    }
}