using EnterpriseLicenseSystem.Domain.Events;
using Microsoft.Extensions.Logging;

namespace EnterpriseLicenseSystem.Application.SoftwareLicenses.EventHandlers;

public class LicenseAssignedEventHandler : INotificationHandler<LicenseAssignedEvent>
{
    private readonly ILogger<LicenseAssignedEventHandler> _logger;

    public LicenseAssignedEventHandler(ILogger<LicenseAssignedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(LicenseAssignedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "EnterpriseLicenseSystem Domain Event: License {LicenseId} assigned to user {UserId} at {AssignedAt}",
            notification.Assignment.SoftwareLicenseId,
            notification.Assignment.UserId,
            notification.Assignment.AssignedAt);

        return Task.CompletedTask;
    }
}

public class LicenseRevokedEventHandler : INotificationHandler<LicenseRevokedEvent>
{
    private readonly ILogger<LicenseRevokedEventHandler> _logger;

    public LicenseRevokedEventHandler(ILogger<LicenseRevokedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(LicenseRevokedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "EnterpriseLicenseSystem Domain Event: License {LicenseId} revoked from user {UserId}",
            notification.Assignment.SoftwareLicenseId,
            notification.Assignment.UserId);

        return Task.CompletedTask;
    }
}
