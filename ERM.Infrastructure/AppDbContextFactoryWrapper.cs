using ERM.Application.Interfaces.Data;
using Microsoft.EntityFrameworkCore;

namespace ERM.Infrastructure
{
    public class AppDbContextFactoryWrapper : IAppDbContextFactory
    {
        private readonly IDbContextFactory<AppDbContext> _efFactory;

        public AppDbContextFactoryWrapper(IDbContextFactory<AppDbContext> efFactory)
        {
            _efFactory = efFactory;
        }

        public async Task<IAppDbContext> CreateDbContextAsync(CancellationToken ct = default)
        {
            // реальный контекст под видом интерфейса
            return await _efFactory.CreateDbContextAsync(ct);
        }
    }
}