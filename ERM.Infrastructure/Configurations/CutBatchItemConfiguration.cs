using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERM.Infrastructure.Configurations
{
    public class CutBatchItemConfiguration : IEntityTypeConfiguration<CutBatchItem>
    {
        public void Configure(EntityTypeBuilder<CutBatchItem> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Color).IsRequired().HasMaxLength(100);
            builder.Property(i => i.Quantity).IsRequired();

            builder.HasOne(i => i.ClothingModel)
                .WithMany()
                .HasForeignKey(i => i.ClothingModelId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}