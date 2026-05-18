using System.Collections.ObjectModel;
using ERM.Application.DTOs;

namespace ERM.UI.ViewModelDTOs
{
    public class CutBatchModelGroup
    {
        public string ModelName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public ObservableCollection<CutBatchItemDto> CutBatchItems { get; set; } = [];
    }
}
