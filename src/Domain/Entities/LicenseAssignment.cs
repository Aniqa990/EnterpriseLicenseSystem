using EnterpriseLicenseSystem.Domain.Common;

namespace EnterpriseLicenseSystem.Domain.Entities;

public class LicenseAssignment : BaseAuditableEntity
{
    public int SoftwareLicenseId { get; set; }
    public SoftwareLicense SoftwareLicense { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
