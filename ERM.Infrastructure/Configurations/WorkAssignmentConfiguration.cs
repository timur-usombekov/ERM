using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERM.Infrastructure.Configurations
{
    public class WorkAssignmentConfiguration : IEntityTypeConfiguration<WorkAssignment>
    {
        public void Configure(EntityTypeBuilder<WorkAssignment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Size).IsRequired().HasMaxLength(20);
            builder.Property(a => a.Quantity).IsRequired();

            builder.HasOne(a => a.Seamstress)
                .WithMany()
                .HasForeignKey(a => a.SeamstressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.CutBatchItem)
                .WithMany()
                .HasForeignKey(a => a.CutBatchItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => new { a.WeekNumber, a.Year });
            builder.HasIndex(a => a.AssignedDate);
        }
    }
}