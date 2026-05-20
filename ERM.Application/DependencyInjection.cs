using ERM.Application.Interfaces.Services;
using ERM.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ERM.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IClothingModelService, ClothingModelService>();
            services.AddTransient<ICutBatchService, CutBatchService>();
            services.AddTransient<IWorkAssignmentService, WorkAssignmentService>();
            services.AddTransient<IFabricColorService, FabricColorService>();
            services.AddTransient<IPayrollService, PayrollService>();

            return services;
        }
    }
}