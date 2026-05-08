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

            builder.Property(m => m.Description)
                .HasMaxLength(1000);

            builder.HasMany(m => m.Assignments)
                .WithOne(a => a.ClothingModel)
                .HasForeignKey(a => a.ClothingModelId)
                .OnDelete(DeleteBehavior.Restrict); // что бы не удалить пошив при удалении модели одежды
        }
    }
}
