using ERM.Application.Interfaces.Data;
using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERM.Infrastructure
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Seamstress> Seamstresses { get; set; } = null!;
        public DbSet<ClothingModel> ClothingModels { get; set; } = null!;
        public DbSet<WorkAssignment> WorkAssignments { get; set; } = null!;
        public DbSet<CutBatch> CutBatches { get; set; } = null!;
        public DbSet<CutBatchItem> CutBatchItems { get; set; } = null!;
        public DbSet<FabricColor> FabricColors { get; set; } = null!;
        public DbSet<PayrollAdjustment> PayrollAdjustments { get; set; } = null!;
        public DbSet<Cutter> Cutters { get; set; } = null!;
        public DbSet<Ironer> Ironers { get; set; } = null!;


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}