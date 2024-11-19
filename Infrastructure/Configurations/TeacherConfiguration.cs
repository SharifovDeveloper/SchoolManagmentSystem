using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

internal sealed class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable("teacher");
        builder.HasKey(t => t.Id);

        #region Relationships

        builder
            .HasOne(t => t.City)
            .WithMany(c => c.Teachers)
            .HasForeignKey(t => t.CityId)
            .HasConstraintName("fk_teacher_city");

        #endregion

        #region Properties

        builder.Property(t => t.Name)
               .HasColumnName("name") 
               .HasMaxLength(Constants.MAX_STRING_LENGTH)
               .IsRequired();

        builder.Property(t => t.BirthDate)
               .HasColumnName("birth_date") 
               .IsRequired();

        builder.Property(t => t.Gender)
               .HasColumnName("gender") 
               .HasConversion<string>() 
               .IsRequired();

        builder.Property(t => t.CreatedDate)
               .HasColumnName("created_date") 
               .IsRequired();

        builder.Property(t => t.LastUpdatedDate)
               .HasColumnName("last_updated_date")
               .IsRequired();

        builder.Property(t => t.IsDeleted)
               .HasColumnName("is_deleted") 
               .HasDefaultValue(false);

        #endregion
    }
}
