using EnterpriseLicenseSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseLicenseSystem.Infrastructure.Data.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Model)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.SerialNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.SerialNumber)
            .IsUnique();

        builder.Property(x => x.AssignedToUserId)
            .HasMaxLength(450);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
