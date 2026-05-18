using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERM.Application.Interfaces.Data
{
    public interface IAppDbContext : IDisposable, IAsyncDisposable
    {
        DbSet<Employee> Employees { get; }
        DbSet<Seamstress> Seamstresses { get; }
        DbSet<ClothingModel> ClothingModels { get; }
        DbSet<WorkAssignment> WorkAssignments { get; }
        DbSet<CutBatch> CutBatches { get; }
        DbSet<CutBatchItem> CutBatchItems { get; }
        DbSet<FabricColor> FabricColors { get; }
        DbSet<PayrollAdjustment> PayrollAdjustments { get; }
        DbSet<Cutter> Cutters { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}