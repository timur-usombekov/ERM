using ERM.Application.Interfaces.Services;
using ERM.UI.ViewModels.Base;

namespace ERM.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEmployeeService _employeeService;

        private ViewModelBase _currentViewModel = null!;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetField(ref _currentViewModel, value);
        }

        public RelayCommand NavigateToEmployeesCommand { get; }

        public MainViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;

            NavigateToEmployeesCommand = new RelayCommand(
                _ => CurrentViewModel = new EmployeesViewModel(_employeeService));

            CurrentViewModel = new EmployeesViewModel(_employeeService);
        }
    }
}