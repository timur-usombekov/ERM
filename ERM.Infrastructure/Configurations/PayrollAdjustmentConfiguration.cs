using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERM.Infrastructure.Configurations
{
    public class PayrollAdjustmentConfiguration : IEntityTypeConfiguration<PayrollAdjustment>
    {
        public void Configure(EntityTypeBuilder<PayrollAdjustment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Date).IsRequired();

            builder.Property(a => a.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(a => a.Reason)
                .IsRequired()
                .HasMaxLength(250);

            builder.HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Индекс по дате для быстрого поиска при расчете ЗП
            builder.HasIndex(a => a.Date);
        }
    }
}