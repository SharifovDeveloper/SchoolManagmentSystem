using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

internal sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("department");
        builder.HasKey(d => d.Id);

        #region Properties

        builder.Property(d => d.Name)
               .HasColumnName("name") 
               .HasMaxLength(Constants.MAX_STRING_LENGTH)
               .IsRequired();

        builder.Property(d => d.CreatedDate)
               .HasColumnName("created_date") 
               .IsRequired();

        builder.Property(d => d.LastUpdatedDate)
               .HasColumnName("last_updated_date") 
               .IsRequired();

        builder.Property(d => d.IsDeleted)
               .HasColumnName("is_deleted")
               .HasDefaultValue(false);

        #endregion
    }
}
