namespace ERM.UI.ViewModels.Dialogs
{
    public class ConfirmDialogViewModel
    {
        public string Message { get; }

        public ConfirmDialogViewModel(string message)
        {
            Message = message;
        }
    }
}