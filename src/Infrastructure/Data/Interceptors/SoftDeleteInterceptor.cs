using EnterpriseLicenseSystem.Domain.Common;
using EnterpriseLicenseSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EnterpriseLicenseSystem.Infrastructure.Data.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    private static readonly Type[] SoftDeleteEntityTypes =
    {
        typeof(SoftwareLicense),
        typeof(Asset)
    };

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ConvertHardDeletesToSoftDeletes(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ConvertHardDeletesToSoftDeletes(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ConvertHardDeletesToSoftDeletes(DbContext? context)
    {
        if (context is null) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State != EntityState.Deleted || !SoftDeleteEntityTypes.Contains(entry.Entity.GetType()))
            {
                continue;
            }

            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
        }
    }
}
