using ERM.Application.Interfaces.Services;
using ERM.Core.Domain.Entities;
using ERM.UI.ViewModels.Base;
using ERM.UI.ViewModels.Dialogs;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace ERM.UI.ViewModels
{
    public class EmployeesViewModel : ViewModelBase
    {
        private readonly IEmployeeService _employeeService;

        public ObservableCollection<Employee> Employees { get; } = [];

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetField(ref _isLoading, value);
        }

        public AsyncRelayCommand LoadEmployeesCommand { get; }
        public AsyncRelayCommand AddEmployeeCommand { get; }
        public AsyncRelayCommand<Employee> DeleteEmployeeCommand { get; }
        public AsyncRelayCommand<Employee> EditEmployeeCommand { get; }


        public EmployeesViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            LoadEmployeesCommand = new AsyncRelayCommand(_ => LoadEmployeesAsync());
            AddEmployeeCommand = new AsyncRelayCommand(_ => AddEmployeeAsync());
            DeleteEmployeeCommand = new AsyncRelayCommand<Employee>(DeleteEmployeeAsync);
            EditEmployeeCommand = new AsyncRelayCommand<Employee>(EditEmployeeAsync);

            _ = LoadEmployeesAsync();
        }
        private async Task EditEmployeeAsync(Employee? employee)
        {
            if (employee is null) return;

            var dialogVm = new EditEmployeeDialogViewModel(employee);
            var result = await DialogHost.Show(dialogVm, "RootDialog");

            if (result is not EditEmployeeDialogViewModel vm || !vm.IsValid)
                return;

            // Передаем всё в сервис ОДНИМ вызовом
            await _employeeService.EditEmployeeAsync(
                vm.EmployeeId,
                vm.FullName,
                vm.PhoneNumber,
                vm.Notes,
                vm.IsSeamstress,
                vm.MachineNumber
            );

            await LoadEmployeesAsync();
        }

        private async Task DeleteEmployeeAsync(Employee? employee)
        {
            if (employee is null) return;

            var confirmed = await DialogHost.Show(
                new ConfirmDialogViewModel($"Удалить сотрудника «{employee.FullName}»?"),
                "RootDialog");

            if (confirmed?.ToString() != "True") return;

            await _employeeService.DeleteAsync(employee.Id);
            Employees.Remove(employee); // не перегружаем весь список — просто убираем из коллекции
        }

        private async Task LoadEmployeesAsync()
        {
            IsLoading = true;
            try
            {
                var employees = await _employeeService.GetAllAsync();
                Employees.Clear();
                foreach (var e in employees)
                    Employees.Add(e);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddEmployeeAsync()
        {
            var dialogVm = new AddEmployeeDialogViewModel();

            var result = await DialogHost.Show(dialogVm, "RootDialog");

            // Пользователь нажал "Отмена" — result будет null
            if (result is not AddEmployeeDialogViewModel vm || !vm.IsValid)
                return;

            await _employeeService.CreateAsync(
                   vm.FullName,
                   vm.PhoneNumber,
                   vm.Notes,
                   vm.IsSeamstress ? vm.MachineNumber : null); // передаём номер машины если швея

            await LoadEmployeesAsync();
        }
    }
}