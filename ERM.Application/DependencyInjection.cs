using ERM.Application.Interfaces.Services;
using ERM.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ERM.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IClothingModelService, ClothingModelService>();
            services.AddScoped<ICutBatchService, CutBatchService>();

            return services;
        }
    }
}