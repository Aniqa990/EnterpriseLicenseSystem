using EnterpriseLicenseSystem.Domain.Constants;
using EnterpriseLicenseSystem.Domain.Entities;
using EnterpriseLicenseSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EnterpriseLicenseSystem.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            // Only run seed logic if relational tables exist
            var databaseCreator = _context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (databaseCreator != null && await databaseCreator.HasTablesAsync())
            {
                await TrySeedAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // 1. Default Roles
        var roles = new[] { Roles.Administrator, Roles.LicenseManager, Roles.Employee };

        foreach (var roleName in roles)
        {
            var role = new IdentityRole(roleName);

            if (!await _roleManager.Roles.AnyAsync(r => r.Name == role.Name))
            {
                await _roleManager.CreateAsync(role);
            }
        }

        // 2. Default Administrator
        var administrator = new ApplicationUser
        {
            UserName = "admin@company.com",
            Email = "admin@company.com",
            EmailConfirmed = true
        };

        if (!await _userManager.Users.AnyAsync(u => u.UserName == administrator.UserName))
        {
            var result = await _userManager.CreateAsync(administrator, "Admin123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(administrator, new[] { Roles.Administrator });
            }
        }

        // 3. Sample Todo Data
        if (!await _context.TodoLists.AnyAsync())
        {
            _context.TodoLists.Add(new TodoList
            {
                Title = "Todo List",
                Items =
                {
                    new TodoItem { Title = "Make a todo list 📃" },
                    new TodoItem { Title = "Check off the first item ✅" },
                    new TodoItem { Title = "Realise you've already done two things on the list! 🤯" },
                    new TodoItem { Title = "Reward yourself with a nice, long nap 🏆" }
                }
            });

            await _context.SaveChangesAsync();
        }
    }
}
