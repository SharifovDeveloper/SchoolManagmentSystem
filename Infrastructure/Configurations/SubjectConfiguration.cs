using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CheckDrive.Infrastructure.Persistence.Configurations;

internal sealed class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable(nameof(Subject));
        builder.HasKey(s => s.Id);

        #region Properties

        builder.Property(s => s.Name)
               .HasMaxLength(Constants.MAX_STRING_LENGTH)
               .IsRequired();

        builder.Property(s => s.GradeLevel)
               .IsRequired();

        builder.Property(s => s.CreatedDate)
               .IsRequired();

        builder.Property(s => s.LastUpdatedDate)
               .IsRequired();

        builder.Property(s => s.IsDeleted)
               .HasDefaultValue(false);

        #endregion
    }
}
