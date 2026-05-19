using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERM.Infrastructure.Configurations
{
    public class IronerConfiguration : IEntityTypeConfiguration<Ironer>
    {
        public void Configure(EntityTypeBuilder<Ironer> builder)
        {
            builder.HasKey(i => i.Id);
        }
    }

}
