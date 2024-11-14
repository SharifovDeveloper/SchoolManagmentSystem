using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable(nameof(City));
        builder.HasKey(c => c.Id);

        #region Properties

        builder.Property(c => c.Name)
            .HasMaxLength(Constants.MAX_STRING_LENGTH)
            .IsRequired();

        builder.Property(c => c.CreatedDate)
            .IsRequired();

        builder.Property(c => c.LastUpdatedDate)
            .IsRequired();

        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        #endregion

    }
}
