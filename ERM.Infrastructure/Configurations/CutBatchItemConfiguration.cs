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

            builder.Property(i => i.Quantity).IsRequired();
            builder.Property(i => i.IssuedQuantity).IsRequired();

            builder.HasOne(i => i.ClothingModel)
                .WithMany()
                .HasForeignKey(i => i.ClothingModelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.FabricColor)
                .WithMany()
                .HasForeignKey(i => i.FabricColorId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}