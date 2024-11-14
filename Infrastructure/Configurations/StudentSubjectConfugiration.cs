using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CheckDrive.Infrastructure.Persistence.Configurations;

internal sealed class StudentSubjectConfiguration : IEntityTypeConfiguration<StudentSubject>
{
    public void Configure(EntityTypeBuilder<StudentSubject> builder)
    {
        builder.ToTable(nameof(StudentSubject));
        builder.HasKey(ss => ss.Id);

        #region Relationships

        builder
            .HasOne(ss => ss.Student)
            .WithMany(s => s.StudentSubjects)
            .HasForeignKey(ss => ss.StudentId);

        builder
            .HasOne(ss => ss.Subject)
            .WithMany(s => s.StudentSubjects)
            .HasForeignKey(ss => ss.SubjectId);

        #endregion

        #region Properties

        builder.Property(ss => ss.Mark)
               .IsRequired();

        builder.Property(ss => ss.CreatedDate)
               .IsRequired();

        builder.Property(ss => ss.LastUpdatedDate)
               .IsRequired();

        builder.Property(ss => ss.IsDeleted)
               .HasDefaultValue(false);

        #endregion
    }
}
