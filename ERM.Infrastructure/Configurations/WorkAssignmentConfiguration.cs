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

            builder.Property(a => a.Size).HasMaxLength(20);
            builder.Property(a => a.Quantity).IsRequired();
            builder.Property(a => a.OperationType).IsRequired();

            builder.Property(a => a.PricePerUnit)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId)
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