using ERM.UI.ViewModels.Dialogs;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ERM.UI.ViewModels.Base
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected async Task<bool> ExecuteSafeAsync(Func<Task> action)
        {
            try
            {
                await action();
                return true; // Успех
            }
            catch (InvalidOperationException ex) //  ошибки бизнес-логики
            {
                await DialogHost.Show(new ErrorDialogViewModel(ex.Message), "RootDialog");
                return false;
            }
            catch (ArgumentException ex) //  ошибки валидации домена
            {
                await DialogHost.Show(new ErrorDialogViewModel(ex.Message), "RootDialog");
                return false;
            }
            catch (Exception ex) //  всё остальное (БД упала и тд)
            {
                await DialogHost.Show(new ErrorDialogViewModel($"Критическая ошибка: {ex.Message}"), "RootDialog");
                return false;
            }
        }

    }
}