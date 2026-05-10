using ERM.UI.ViewModels.Base;

namespace ERM.UI.ViewModels.Dialogs
{
    public class AddCutBatchDialogViewModel : ViewModelBase
    {
        private string _title = $"Крой от {DateTime.Now:dd.MM.yyyy}";
        public string Title
        {
            get => _title;
            set
            {
                SetField(ref _title, value);
                OnPropertyChanged(nameof(IsValid));
            }
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
            set
            {
                SetField(ref _declaredQuantityText, value);
                OnPropertyChanged(nameof(DeclaredQuantity));
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public int DeclaredQuantity => int.TryParse(DeclaredQuantityText, out var q) ? q : 0;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(Title) &&
            DeclaredQuantity > 0;
    }
}