
using EnterpriseLicenseSystem.Domain.Entities;

namespace EnterpriseLicenseSystem.Application.SoftwareLicenses.Queries;
public record SoftwareLicenseBriefDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string LicenseKey { get; init; } = string.Empty;
    public int TotalSeats { get; init; }
    public int AllocatedSeats { get; init; }
    public int AvailableSeats => TotalSeats - AllocatedSeats;
    public DateTime ExpirationDate { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<SoftwareLicense, SoftwareLicenseBriefDto>();
        }
    }
}
