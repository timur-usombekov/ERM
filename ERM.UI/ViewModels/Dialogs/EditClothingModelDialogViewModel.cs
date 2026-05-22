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

        private string _article;
        public string Article
        {
            get => _article;
            set => SetField(ref _article, value);
        }

        private string _sewingPriceText = string.Empty;
        public string SewingPriceText
        {
            get => _sewingPriceText;
            set { SetField(ref _sewingPriceText, value); OnPropertyChanged(nameof(IsValid)); }
        }

        private string _ironingPriceText = string.Empty;
        public string IroningPriceText
        {
            get => _ironingPriceText;
            set { SetField(ref _ironingPriceText, value); OnPropertyChanged(nameof(IsValid)); }
        }

        private string? _description;
        public string? Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(Name) &&
            !string.IsNullOrWhiteSpace(Article) &&
            decimal.TryParse(SewingPriceText, out var p) && p >= 0 &&
            decimal.TryParse(IroningPriceText, out var ir) && ir >= 0;


        public EditClothingModelDialogViewModel(ClothingModelDto dto)
        {
            ModelId = dto.Id;
            _name = dto.Name;
            _article = dto.Article;
            _sewingPriceText = dto.SewingPrice.ToString();
            _ironingPriceText = dto.IroningPrice.ToString();
            _description = dto.Description;
        }
    }
}
