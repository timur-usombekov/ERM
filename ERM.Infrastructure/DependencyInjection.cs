using ERM.Application.Interfaces.Data;
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

            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));
            services.AddSingleton<IAppDbContextFactory, AppDbContextFactoryWrapper>();


            return services;
        }

    }
}