namespace EnterpriseLicenseSystem.Domain.Events;

public class LicenseAssignedEvent : BaseEvent
{
    public LicenseAssignedEvent(LicenseAssignment assignment)
    {
        Assignment = assignment;
    }

    public LicenseAssignment Assignment { get; }
}
