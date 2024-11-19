using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

internal sealed class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("subject");
        builder.HasKey(s => s.Id);

        #region Properties

        builder.Property(s => s.Name)
               .HasColumnName("name") 
               .HasMaxLength(Constants.MAX_STRING_LENGTH)
               .IsRequired();

        builder.Property(s => s.GradeLevel)
               .HasColumnName("grade_level") 
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
