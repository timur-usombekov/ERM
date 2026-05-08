using ERM.UI.ViewModels.Base;

namespace ERM.UI.ViewModels.Dialogs
{
    public class AddClothingModelDialogViewModel : ViewModelBase
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private string? _description;
        public string? Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(Name);
    }
}
