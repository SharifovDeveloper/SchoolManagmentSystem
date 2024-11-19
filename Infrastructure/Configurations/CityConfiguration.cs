using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("city");      
        builder.HasKey(c => c.Id);

        #region Properties

        builder.Property(c => c.Name)
            .HasColumnName("name") 
            .HasMaxLength(Constants.MAX_STRING_LENGTH)
            .IsRequired();

        builder.Property(c => c.CreatedDate)
            .HasColumnName("created_date") 
            .IsRequired();

        builder.Property(c => c.LastUpdatedDate)
            .HasColumnName("last_updated_date") 
            .IsRequired();

        builder.Property(c => c.IsDeleted)
            .HasColumnName("is_deleted") 
            .HasDefaultValue(false);

        #endregion
    }
}
