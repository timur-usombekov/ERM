using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERM.Infrastructure.Configurations
{
    public class SeamstressConfiguration : IEntityTypeConfiguration<Seamstress>
    {
        public void Configure(EntityTypeBuilder<Seamstress> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.MachineNumber)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}