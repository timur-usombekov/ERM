using ERM.Application.DTOs;
using ERM.UI.ViewModels.Base;
using System.Collections.ObjectModel;

namespace ERM.UI.ViewModels.Dialogs
{
    public class EditCutBatchDialogViewModel : ViewModelBase
    {
        public ObservableCollection<EmployeeDto> Cutters { get; }

        private EmployeeDto? _selectedCutter;
        public EmployeeDto? SelectedCutter
        {
            get => _selectedCutter;
            set { SetField(ref _selectedCutter, value); OnPropertyChanged(nameof(IsValid)); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set { SetField(ref _title, value); OnPropertyChanged(nameof(IsValid)); }
        }

        private DateTime _date = DateTime.Today;
        public DateTime Date
        {
            get => _date;
            set => SetField(ref _date, value);
        }

        private string _declaredQuantityText = string.Empty;
        public string DeclaredQuantityText
        {
            get => _declaredQuantityText;
            set { SetField(ref _declaredQuantityText, value); OnPropertyChanged(nameof(DeclaredQuantity)); OnPropertyChanged(nameof(IsValid)); }
        }

        public int DeclaredQuantity => int.TryParse(DeclaredQuantityText, out var q) ? q : 0;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(Title) &&
            DeclaredQuantity > 0 &&
            SelectedCutter != null;

        public EditCutBatchDialogViewModel(CutBatchDto existingBatch, IEnumerable<EmployeeDto> allEmployees)
        {
            Cutters = new ObservableCollection<EmployeeDto>(allEmployees.Where(e => e.IsCutter));

            Title = existingBatch.Title;
            Date = existingBatch.Date.ToDateTime(TimeOnly.MinValue);
            DeclaredQuantityText = existingBatch.DeclaredQuantity.ToString();

            // Нужно будет добавить это в DTO и передавать сюда, чтобы точно отображать существующего закройщика при открытии диалога редактирования
            // SelectedCutter = Cutters.FirstOrDefault(c => c.Id == existingBatch.CutterEmployeeId);

            SelectedCutter = Cutters.FirstOrDefault(); // Заглушка потому что закройщик один 99% случаев
        }
    }
}