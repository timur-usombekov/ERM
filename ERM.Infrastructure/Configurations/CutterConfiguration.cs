using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERM.Infrastructure.Configurations
{
    public class CutterConfiguration : IEntityTypeConfiguration<Cutter>
    {
        public void Configure(EntityTypeBuilder<Cutter> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Percentage).IsRequired().HasColumnType("decimal(5,2)");
        }
    }
}