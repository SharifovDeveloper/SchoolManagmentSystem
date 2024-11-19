using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

internal sealed class TeacherSubjectConfiguration : IEntityTypeConfiguration<TeacherSubject>
{
    public void Configure(EntityTypeBuilder<TeacherSubject> builder)
    {
        builder.ToTable("teacher_subject");
        builder.HasKey(ts => ts.Id);

        #region Relationships

        builder
            .HasOne(ts => ts.Teacher)
            .WithMany(t => t.TeacherSubjects)
            .HasForeignKey(ts => ts.TeacherId)
            .HasConstraintName("fk_teacher_subject_teacher");

        builder
            .HasOne(ts => ts.Subject)
            .WithMany(s => s.TeacherSubjects)
            .HasForeignKey(ts => ts.SubjectId)
            .HasConstraintName("fk_teacher_subject_subject");

        #endregion

        #region Properties

        builder.Property(ts => ts.CreatedDate)
               .HasColumnName("created_date") 
               .IsRequired();

        builder.Property(ts => ts.LastUpdatedDate)
               .HasColumnName("last_updated_date") 
               .IsRequired();

        builder.Property(ts => ts.IsDeleted)
               .HasColumnName("is_deleted") 
               .HasDefaultValue(false);

        #endregion
    }
}
