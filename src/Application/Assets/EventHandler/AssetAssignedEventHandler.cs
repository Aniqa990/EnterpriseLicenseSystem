using EnterpriseLicenseSystem.Domain.Events;
using Microsoft.Extensions.Logging;

namespace EnterpriseLicenseSystem.Application.Assets.EventHandlers;

public class AssetAssignedEventHandler : INotificationHandler<AssetAssignedEvent>
{
    private readonly ILogger<AssetAssignedEventHandler> _logger;

    public AssetAssignedEventHandler(ILogger<AssetAssignedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(AssetAssignedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "EnterpriseLicenseSystem Domain Event: Asset {AssetId} ({SerialNumber}) assigned to user {UserId}",
            notification.Asset.Id, notification.Asset.SerialNumber, notification.Asset.AssignedToUserId);

        return Task.CompletedTask;
    }
}
