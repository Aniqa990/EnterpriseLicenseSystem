namespace EnterpriseLicenseSystem.Domain.Events;

public class AssetAssignedEvent : BaseEvent
{
    public AssetAssignedEvent(Asset asset)
    {
        Asset = asset;
    }

    public Asset Asset { get; }
}
