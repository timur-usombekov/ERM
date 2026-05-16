using ERM.UI.ViewModels.Base;

namespace ERM.UI.ViewModels.Dialogs
{
    public class AddClothingModelDialogViewModel : ViewModelBase
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                SetField(ref _name, value);
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private string _sewingPriceText = string.Empty;
        public string SewingPriceText
        {
            get => _sewingPriceText;
            set { SetField(ref _sewingPriceText, value); OnPropertyChanged(nameof(IsValid)); }
        }

        private string? _description;
        public string? Description
        {
            get => _description;
            set
            {
                SetField(ref _description, value);
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(Name) &&
            decimal.TryParse(SewingPriceText, out var p) && p >= 0;

    }
}
