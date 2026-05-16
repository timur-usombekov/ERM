using System.Collections.ObjectModel;

namespace ERM.UI.ViewModelDTOs
{
    public class CutBatchModelGroup
    {
        public string ModelName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public ObservableCollection<CutBatchItemDto> CutBatchItems { get; set; } = [];
    }
}
