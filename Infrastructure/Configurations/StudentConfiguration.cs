using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

internal sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("student");
        builder.HasKey(s => s.Id);

        #region Relationships

        builder
            .HasOne(s => s.City)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.CityId)
            .HasConstraintName("fk_student_city");

        builder
            .HasOne(s => s.Department)
            .WithMany(d => d.Students)
            .HasForeignKey(s => s.DepartmentId)
            .HasConstraintName("fk_student_department");

        #endregion

        #region Properties

        builder.Property(s => s.Name)
               .HasColumnName("name") 
               .HasMaxLength(Constants.MAX_STRING_LENGTH)
               .IsRequired();

        builder.Property(s => s.BirthDate)
               .HasColumnName("birth_date") 
               .IsRequired();

        builder.Property(s => s.Gender)
               .HasColumnName("gender") 
               .HasConversion<string>() 
               .IsRequired();

        builder.Property(s => s.CurrentGradeLevel)
               .HasColumnName("current_grade_level") 
               .IsRequired();

        builder.Property(s => s.CreatedDate)
               .HasColumnName("created_date") 
               .IsRequired();

        builder.Property(s => s.LastUpdatedDate)
               .HasColumnName("last_updated_date") 
               .IsRequired();

        builder.Property(s => s.IsDeleted)
               .HasColumnName("is_deleted") 
               .HasDefaultValue(false);

        #endregion
    }
}
