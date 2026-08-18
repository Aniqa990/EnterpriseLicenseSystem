using System.ComponentModel.DataAnnotations;
using EnterpriseLicenseSystem.Domain.Common;

namespace EnterpriseLicenseSystem.Domain.Entities;

public class SoftwareLicense : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string LicenseKey { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public int AllocatedSeats { get; set; }
    public DateTime ExpirationDate { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public ICollection<LicenseAssignment> Assignments { get; set; } = new List<LicenseAssignment>();

}
