using ERM.UI.ViewModels.Base;

namespace ERM.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel = null!;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetField(ref _currentViewModel, value);
        }

        public AsyncRelayCommand NavigateToEmployeesCommand { get; }
        public AsyncRelayCommand NavigateToClothingModelsCommand { get; }
        public AsyncRelayCommand NavigateToCutBatchesCommand { get; }
        public AsyncRelayCommand NavigateToDashboardCommand { get; }
        public AsyncRelayCommand NavigateToPayrollCommand { get; }

        public MainViewModel(
            Func<EmployeesViewModel> employeesVmFactory,
            Func<ClothingModelsViewModel> clothingModelsVmFactory,
            Func<CutBatchesViewModel> cutBatchesVmFactory,
            Func<DashboardViewModel> dashboardVmFactory,
            Func<PayrollViewModel> payrollVmFactory)
        {
            NavigateToEmployeesCommand = new AsyncRelayCommand(_ => NavigateAsync(employeesVmFactory()));
            NavigateToClothingModelsCommand = new AsyncRelayCommand(_ => NavigateAsync(clothingModelsVmFactory()));
            NavigateToCutBatchesCommand = new AsyncRelayCommand(_ => NavigateAsync(cutBatchesVmFactory()));
            NavigateToDashboardCommand = new AsyncRelayCommand(_ => NavigateAsync(dashboardVmFactory()));
            NavigateToPayrollCommand = new AsyncRelayCommand(async _ => await NavigateAsync(payrollVmFactory()));

            _ = NavigateAsync(dashboardVmFactory());
        }

        private async Task NavigateAsync(ViewModelBase viewModel)
        {
            CurrentViewModel = viewModel;

            // Если ViewModel поддерживает обновление при навигации — обновляем её данные желательно не теряя контекста (например, выбранного элемента)
            if (CurrentViewModel is INavigationAware navAwareVM)
            {
                await navAwareVM.OnNavigatedToAsync();
            }
        }

    }
}