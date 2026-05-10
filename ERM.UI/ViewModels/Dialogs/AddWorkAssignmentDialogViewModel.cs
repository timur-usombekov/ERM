using ERM.Application.DTOs;
using ERM.UI.ViewModels.Base;
using System.Collections.ObjectModel;

namespace ERM.UI.ViewModels.Dialogs
{
    public class AddWorkAssignmentDialogViewModel : ViewModelBase
    {
        public ObservableCollection<EmployeeDto> Seamstresses { get; }
        public ObservableCollection<CutBatchItemDto> CutBatchItems { get; }

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

        private string _size = string.Empty;
        public string Size
        {
            get => _size;
            set
            {
                SetField(ref _size, value);
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
            !string.IsNullOrWhiteSpace(Size) &&
            Quantity > 0;

        public AddWorkAssignmentDialogViewModel(IEnumerable<EmployeeDto> seamstresses, IEnumerable<CutBatchItemDto> cutBatchItems)
        {
            Seamstresses = new ObservableCollection<EmployeeDto>(seamstresses);
            CutBatchItems = new ObservableCollection<CutBatchItemDto>(cutBatchItems);
        }
    }
}