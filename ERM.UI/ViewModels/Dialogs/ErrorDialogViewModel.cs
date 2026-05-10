namespace ERM.UI.ViewModels.Dialogs
{
    public class ErrorDialogViewModel
    {
        public string ErrorMessage { get; }

        public ErrorDialogViewModel(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}