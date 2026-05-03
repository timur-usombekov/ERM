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
            builder.Property(a => a.Color).HasMaxLength(50);

            builder.HasOne(a => a.Seamstress)
                .WithMany()
                .HasForeignKey(a => a.SeamstressId)
                .OnDelete(DeleteBehavior.Restrict); // Что бы случайно не удалить выполненый пошив

            builder.HasIndex(a => new { a.WeekNumber, a.Year }); // частый запрос — индекс
        }
    }
}
