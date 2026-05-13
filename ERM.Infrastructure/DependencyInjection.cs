using ERM.Application.Interfaces.Repositories;
using ERM.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ERM.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ERM",
                "erm.db"
            );

            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IClothingModelRepository, ClothingModelRepository>();
            services.AddScoped<ICutBatchRepository, CutBatchRepository>();
            services.AddScoped<IWorkAssignmentRepository, WorkAssignmentRepository>();
            services.AddScoped<IFabricColorRepository, FabricColorRepository>();

            return services;
        }
    }
}