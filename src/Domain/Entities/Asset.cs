using EnterpriseLicenseSystem.Domain.Common;

namespace EnterpriseLicenseSystem.Domain.Entities;

public class Asset : BaseAuditableEntity
{
    public string Model { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string? AssignedToUserId { get; set; }
}
