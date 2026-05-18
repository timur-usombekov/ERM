using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERM.Infrastructure.Configurations
{
    public class CutBatchConfiguration : IEntityTypeConfiguration<CutBatch>
    {
        public void Configure(EntityTypeBuilder<CutBatch> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title).IsRequired().HasMaxLength(200);
            builder.Property(b => b.Date).IsRequired();
            builder.Property(b => b.IsClosed).IsRequired();

            builder.HasMany(b => b.Items)
                .WithOne(i => i.CutBatch)
                .HasForeignKey(i => i.CutBatchId)
                .OnDelete(DeleteBehavior.Cascade); // Пока что каскадное удаление от кроя, можно изменить при необходимости

            builder.HasOne(b => b.CutterEmployee)
                .WithMany()
                .HasForeignKey(b => b.CutterEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(b => b.DeclaredQuantity).IsRequired();
        }
    }
}