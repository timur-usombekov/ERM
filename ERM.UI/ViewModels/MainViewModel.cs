using ERM.Application.Interfaces.Services;
using ERM.Application.Services;
using ERM.UI.ViewModels.Base;

namespace ERM.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IClothingModelService _clothingModelService;
        private readonly ICutBatchService _cutBatchService;


        private ViewModelBase _currentViewModel = null!;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetField(ref _currentViewModel, value);
        }

        public RelayCommand NavigateToEmployeesCommand { get; }
        public RelayCommand NavigateToClothingModelsCommand { get; }
        public RelayCommand NavigateToCutBatchesCommand { get; }

        public MainViewModel(
            IEmployeeService employeeService, 
            IClothingModelService clothingModelService, 
            ICutBatchService cutBatchService)
        {
            _employeeService = employeeService;
            _clothingModelService = clothingModelService;
            _cutBatchService = cutBatchService;

            NavigateToEmployeesCommand = new RelayCommand(_ =>
                CurrentViewModel = new EmployeesViewModel(_employeeService));

            NavigateToClothingModelsCommand = new RelayCommand(_ =>
                CurrentViewModel = new ClothingModelsViewModel(_clothingModelService));

            NavigateToCutBatchesCommand = new RelayCommand(_ => 
                CurrentViewModel = new CutBatchesViewModel(_cutBatchService, _clothingModelService));


            CurrentViewModel = new EmployeesViewModel(_employeeService);
        }
    }
}
