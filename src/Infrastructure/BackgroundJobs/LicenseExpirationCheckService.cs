using EnterpriseLicenseSystem.Application.Common.Interfaces;
using EnterpriseLicenseSystem.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EnterpriseLicenseSystem.Infrastructure.BackgroundJobs;

public class LicenseExpirationCheckService : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(24);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LicenseExpirationCheckService> _logger;
    private readonly IOptionsMonitor<LicenseSettings> _settings;

    public LicenseExpirationCheckService(
        IServiceScopeFactory scopeFactory,
        ILogger<LicenseExpirationCheckService> logger,
        IOptionsMonitor<LicenseSettings> settings)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _settings = settings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckExpiringLicensesAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "EnterpriseLicenseSystem: License expiration check failed.");
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }
    }

    private async Task CheckExpiringLicensesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var warningDate = DateTime.UtcNow.AddDays(_settings.CurrentValue.ExpirationWarningDays);

        var expiringLicenses = await context.SoftwareLicenses
            .Where(l => l.ExpirationDate <= warningDate && l.ExpirationDate > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var license in expiringLicenses)
        {
            _logger.LogWarning(
                "EnterpriseLicenseSystem Alert: License {LicenseName} ({LicenseKey}) expires on {ExpirationDate:d}",
                license.Name, license.LicenseKey, license.ExpirationDate);
        }
    }
}
