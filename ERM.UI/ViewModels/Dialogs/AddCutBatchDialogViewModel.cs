using ERM.Application.DTOs;
using ERM.UI.ViewModels.Base;
using System.Collections.ObjectModel;

namespace ERM.UI.ViewModels.Dialogs
{
    public class AddCutBatchDialogViewModel : ViewModelBase
    {
        public ObservableCollection<EmployeeDto> Cutters { get; }

        private EmployeeDto? _selectedCutter;
        public EmployeeDto? SelectedCutter
        {
            get => _selectedCutter;
            set { SetField(ref _selectedCutter, value); OnPropertyChanged(nameof(IsValid)); }
        }

        private string _title = $"Крой от {DateTime.Now:dd.MM.yyyy}";
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

        public AddCutBatchDialogViewModel(IEnumerable<EmployeeDto> allEmployees)
        {
            var cutters = allEmployees.Where(e => e.IsCutter).ToList();
            Cutters = new ObservableCollection<EmployeeDto>(cutters);

            // Если закройщик только один, выбираем его сразу
            if (cutters.Count == 1)
            {
                SelectedCutter = cutters.First();
            }
        }
    }
}