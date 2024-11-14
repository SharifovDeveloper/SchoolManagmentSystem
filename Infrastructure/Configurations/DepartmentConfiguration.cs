using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

internal sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable(nameof(Department));
        builder.HasKey(d => d.Id);

        #region Properties

        builder.Property(d => d.Name)
               .HasMaxLength(Constants.MAX_STRING_LENGTH)
               .IsRequired();

        builder.Property(d => d.CreatedDate)
               .IsRequired();

        builder.Property(d => d.LastUpdatedDate)
               .IsRequired();

        builder.Property(d => d.IsDeleted)
               .HasDefaultValue(false);

        #endregion
    }
}
