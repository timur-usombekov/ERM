namespace ERM.UI.ViewModels.Dialogs
{
    public class ConfirmDialogViewModel
    {
        public string Message { get; }
        public string ConfirmButtonText { get; }
        public bool IsDestructive { get; }

        public ConfirmDialogViewModel(string message, string confirmButtonText = "Удалить", bool isDestructive = true)
        {
            Message = message;
            ConfirmButtonText = confirmButtonText;
            IsDestructive = isDestructive;
        }
    }
}