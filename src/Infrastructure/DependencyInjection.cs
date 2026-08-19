using EnterpriseLicenseSystem.Application.Common.Interfaces;
using EnterpriseLicenseSystem.Application.Common.Models;
using EnterpriseLicenseSystem.Domain.Constants;
using EnterpriseLicenseSystem.Infrastructure.BackgroundJobs;
using EnterpriseLicenseSystem.Infrastructure.Data;
using EnterpriseLicenseSystem.Infrastructure.Data.Interceptors;
using EnterpriseLicenseSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("EnterpriseLicenseSystemDb");
        Guard.Against.Null(connectionString, message: "Connection string 'EnterpriseLicenseSystemDb' not found.");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();

        builder.Services.AddOptions<LicenseSettings>()
            .Bind(builder.Configuration.GetSection(LicenseSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddHostedService<LicenseExpirationCheckService>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString);
            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });


        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.AddAuthorizationBuilder();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints()
            .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>>();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();

        builder.Services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));
    }
}
