using System.Reflection;
using EnterpriseLicenseSystem.Application.Common.Interfaces;
using EnterpriseLicenseSystem.Domain.Entities;
using EnterpriseLicenseSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseLicenseSystem.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public DbSet<SoftwareLicense> SoftwareLicenses => Set<SoftwareLicense>();

    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<LicenseAssignment> LicenseAssignments => Set<LicenseAssignment>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    
    }
}
