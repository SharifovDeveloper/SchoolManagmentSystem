using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CheckDrive.Infrastructure.Persistence.Configurations;

internal sealed class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable(nameof(Teacher));
        builder.HasKey(t => t.Id);

        #region Relationships

        builder
            .HasOne(t => t.City)
            .WithMany(c => c.Teachers)
            .HasForeignKey(t => t.CityId);

        #endregion

        #region Properties

        builder.Property(t => t.Name)
               .HasMaxLength(Constants.MAX_STRING_LENGTH)
               .IsRequired();

        builder.Property(t => t.BirthDate)
               .IsRequired();

        builder.Property(t => t.Gender)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(t => t.CreatedDate)
               .IsRequired();

        builder.Property(t => t.LastUpdatedDate)
               .IsRequired();

        builder.Property(t => t.IsDeleted)
               .HasDefaultValue(false);

        #endregion
    }
}
