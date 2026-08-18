using EnterpriseLicenseSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseLicenseSystem.Infrastructure.Data.Configurations;

public class SoftwareLicenseConfiguration : IEntityTypeConfiguration<SoftwareLicense>
{
    public void Configure(EntityTypeBuilder<SoftwareLicense> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.LicenseKey)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.LicenseKey)
            .IsUnique();

        builder.Property(x => x.TotalSeats)
            .IsRequired();

        builder.Property(x => x.AllocatedSeats)
            .IsRequired();

        builder.Property(x => x.ExpirationDate)
            .IsRequired();

        // ignores soft-deleted licenses globally
        builder.HasQueryFilter(x => !x.IsDeleted);

        //concurrency handling. RowVersion as an optimistic concurrency token
        builder.Property(l => l.RowVersion)
        .IsRowVersion();
    }
}
