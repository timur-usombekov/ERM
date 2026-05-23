using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERM.Infrastructure.Configurations
{
    public class ClothingModelConfiguration : IEntityTypeConfiguration<ClothingModel>
    {
        public void Configure(EntityTypeBuilder<ClothingModel> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.Article)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.SewingPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)"); // формат для денег

            builder.Property(m => m.Description)
                .HasMaxLength(1000);

            builder.Property(m => m.IsDeleted)
                .IsRequired();
        }
    }
}
