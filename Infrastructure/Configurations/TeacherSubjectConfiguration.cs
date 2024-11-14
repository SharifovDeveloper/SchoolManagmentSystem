using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CheckDrive.Infrastructure.Persistence.Configurations;

internal sealed class TeacherSubjectConfiguration : IEntityTypeConfiguration<TeacherSubject>
{
    public void Configure(EntityTypeBuilder<TeacherSubject> builder)
    {
        builder.ToTable(nameof(TeacherSubject));
        builder.HasKey(ts => ts.Id);

        #region Relationships

        builder
            .HasOne(ts => ts.Teacher)
            .WithMany(t => t.TeacherSubjects)
            .HasForeignKey(ts => ts.TeacherId);

        builder
            .HasOne(ts => ts.Subject)
            .WithMany(s => s.TeacherSubjects)
            .HasForeignKey(ts => ts.SubjectId);

        #endregion

        #region Properties

        builder.Property(ts => ts.CreatedDate)
               .IsRequired();

        builder.Property(ts => ts.LastUpdatedDate)
               .IsRequired();

        builder.Property(ts => ts.IsDeleted)
               .HasDefaultValue(false);

        #endregion
    }
}
