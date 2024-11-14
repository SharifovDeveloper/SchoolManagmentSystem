using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CheckDrive.Infrastructure.Persistence.Configurations;

internal sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable(nameof(Student));
        builder.HasKey(s => s.Id);

        #region Relationships

        builder
            .HasOne(s => s.City)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.CityId);

        builder
            .HasOne(s => s.Department)
            .WithMany(d => d.Students)
            .HasForeignKey(s => s.DepartmentId);

        #endregion

        #region Properties

        builder.Property(s => s.Name)
               .HasMaxLength(Constants.MAX_STRING_LENGTH)
               .IsRequired();

        builder.Property(s => s.BirthDate)
               .IsRequired();

        builder.Property(s => s.Gender)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(s => s.CurrentGradeLevel)
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