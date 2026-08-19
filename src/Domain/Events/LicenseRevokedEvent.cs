namespace EnterpriseLicenseSystem.Domain.Events;

public class LicenseRevokedEvent : BaseEvent
{
    public LicenseRevokedEvent(LicenseAssignment assignment)
    {
        Assignment = assignment;
    }

    public LicenseAssignment Assignment { get; }
}
