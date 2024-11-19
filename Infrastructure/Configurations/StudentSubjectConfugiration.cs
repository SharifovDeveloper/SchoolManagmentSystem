using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

internal sealed class StudentSubjectConfiguration : IEntityTypeConfiguration<StudentSubject>
{
    public void Configure(EntityTypeBuilder<StudentSubject> builder)
    {
        builder.ToTable("student_subject");
        builder.HasKey(ss => ss.Id);

        #region Relationships

        builder
            .HasOne(ss => ss.Student)
            .WithMany(s => s.StudentSubjects)
            .HasForeignKey(ss => ss.StudentId)
            .HasConstraintName("fk_student_subject_student");

        builder
            .HasOne(ss => ss.Subject)
            .WithMany(s => s.StudentSubjects)
            .HasForeignKey(ss => ss.SubjectId)
            .HasConstraintName("fk_student_subject_subject");

        #endregion

        #region Properties

        builder.Property(ss => ss.Mark)
               .HasColumnName("mark") 
               .IsRequired();

        builder.Property(ss => ss.CreatedDate)
               .HasColumnName("created_date") 
               .IsRequired();

        builder.Property(ss => ss.LastUpdatedDate)
               .HasColumnName("last_updated_date") 
               .IsRequired();

        builder.Property(ss => ss.IsDeleted)
               .HasColumnName("is_deleted") 
               .HasDefaultValue(false);

        #endregion
    }
}
