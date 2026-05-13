using ERM.Application;
using ERM.Infrastructure;
using ERM.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using ERM.Application.Interfaces.Data;
using Microsoft.EntityFrameworkCore;

namespace ERM.UI
{
    public partial class App : System.Windows.Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddInfrastructure();
                    services.AddApplication();

                    services.AddTransient<MainWindow>();
                    services.AddTransient<MainViewModel>();

                    // все ViewModels как Singleton что бы не забывать контекст
                    services.AddSingleton<EmployeesViewModel>();
                    services.AddSingleton<ClothingModelsViewModel>();
                    services.AddSingleton<CutBatchesViewModel>();
                    services.AddSingleton<DashboardViewModel>();

                    // фабрики (Func). Что бы DI мог отдавать новые вьюмодели по запросу
                    services.AddSingleton<Func<EmployeesViewModel>>(sp => () => sp.GetRequiredService<EmployeesViewModel>());
                    services.AddSingleton<Func<ClothingModelsViewModel>>(sp => () => sp.GetRequiredService<ClothingModelsViewModel>());
                    services.AddSingleton<Func<CutBatchesViewModel>>(sp => () => sp.GetRequiredService<CutBatchesViewModel>());
                    services.AddSingleton<Func<DashboardViewModel>>(sp => () => sp.GetRequiredService<DashboardViewModel>());
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            // Безопасное применение миграций через фабрику
            using var scope = _host.Services.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IAppDbContextFactory>();
            await using (var context = await factory.CreateDbContextAsync())
            {
                // Приводим интерфейс к конкретному типу для вызова Migrate
                if (context is DbContext efContext)
                {
                    await efContext.Database.MigrateAsync();
                }
            }

            _host.Services.GetRequiredService<MainWindow>().Show();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.StopAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
            _host.Dispose();
            base.OnExit(e);
        }
    }
}