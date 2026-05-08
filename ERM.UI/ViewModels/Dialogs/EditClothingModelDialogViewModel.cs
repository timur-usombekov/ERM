using ERM.Application.DTOs;
using ERM.UI.ViewModels.Base;

namespace ERM.UI.ViewModels.Dialogs
{
    public class EditClothingModelDialogViewModel : ViewModelBase
    {
        public Guid ModelId { get; }

        private string _name;
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

        public EditClothingModelDialogViewModel(ClothingModelDto dto)
        {
            ModelId = dto.Id;
            _name = dto.Name;
            _description = dto.Description;
        }
    }
}
