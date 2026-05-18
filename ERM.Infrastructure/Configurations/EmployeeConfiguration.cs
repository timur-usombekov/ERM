using ERM.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERM.Infrastructure.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.Notes)
                .HasMaxLength(1000);

            builder.HasOne(e => e.Seamstress)
                .WithOne(s => s.Employee)
                .HasForeignKey<Seamstress>(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Cutter)
                .WithOne(c => c.Employee)
                .HasForeignKey<Cutter>(c => c.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}