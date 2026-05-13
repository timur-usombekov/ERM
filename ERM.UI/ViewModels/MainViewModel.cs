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

        public RelayCommand NavigateToEmployeesCommand { get; }
        public RelayCommand NavigateToClothingModelsCommand { get; }
        public RelayCommand NavigateToCutBatchesCommand { get; }
        public RelayCommand NavigateToDashboardCommand { get; }

        public MainViewModel(
            Func<EmployeesViewModel> employeesVmFactory,
            Func<ClothingModelsViewModel> clothingModelsVmFactory,
            Func<CutBatchesViewModel> cutBatchesVmFactory,
            Func<DashboardViewModel> dashboardVmFactory)
        {
            NavigateToEmployeesCommand = new RelayCommand(_ => CurrentViewModel = employeesVmFactory());
            NavigateToClothingModelsCommand = new RelayCommand(_ => CurrentViewModel = clothingModelsVmFactory());
            NavigateToCutBatchesCommand = new RelayCommand(_ => CurrentViewModel = cutBatchesVmFactory());
            NavigateToDashboardCommand = new RelayCommand(_ => CurrentViewModel = dashboardVmFactory());

            //  дефолтный экран при запуске
            CurrentViewModel = dashboardVmFactory();
        }
    }
}