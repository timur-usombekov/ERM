using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERM.Infrastructure.Configurations
{
    public class IroningAssignmentConfiguration : IEntityTypeConfiguration<IroningAssignment>
    {
        public void Configure(EntityTypeBuilder<IroningAssignment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.PricePerUnit)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(a => a.Quantity).IsRequired();
            builder.Property(a => a.AssignedDate).IsRequired();

            builder.HasOne(a => a.Ironer)
                .WithMany()
                .HasForeignKey(a => a.IronerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.SubstituteSeamstress)
                .WithMany()
                .HasForeignKey(a => a.SubstituteSeamstressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.CutBatchItem)
                .WithMany()
                .HasForeignKey(a => a.CutBatchItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => new { a.WeekNumber, a.Year });
            builder.HasIndex(a => a.AssignedDate);
            builder.HasIndex(a => a.IronerId);
        }
    }

}
