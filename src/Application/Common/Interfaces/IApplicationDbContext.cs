using EnterpriseLicenseSystem.Domain.Entities;

namespace EnterpriseLicenseSystem.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }

    DbSet<SoftwareLicense> SoftwareLicenses { get; }

    DbSet<LicenseAssignment> LicenseAssignments { get; }

    DbSet<Asset> Assets { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
