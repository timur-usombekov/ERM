namespace ERM.Application.Interfaces.Data
{
    public interface IAppDbContextFactory
    {
        Task<IAppDbContext> CreateDbContextAsync(CancellationToken ct = default);
    }
}