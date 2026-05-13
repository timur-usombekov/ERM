using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERM.Infrastructure.Configurations
{
    public class FabricColorConfiguration : IEntityTypeConfiguration<FabricColor>
    {
        public void Configure(EntityTypeBuilder<FabricColor> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(c => c.Name).IsUnique(); // Защита от дублей на уровне БД
        }
    }
}