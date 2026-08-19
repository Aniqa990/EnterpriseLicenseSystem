using EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands.AssignLicenseCommand;
using EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands.CreateSoftwareLicenseCommand;
using EnterpriseLicenseSystem.Domain.Entities;

namespace EnterpriseLicenseSystem.Application.FunctionalTests.SoftwareLicenses.Commands;

using static Testing;

public class AssignLicenseTests : BaseTestFixture
{
    [Test]
    public async Task ShouldAssignSeatSuccessfully()
    {
        await RunAsDefaultUserAsync();

        var licenseId = await SendAsync(new CreateSoftwareLicenseCommand
        {
            Name = "JetBrains ReSharper",
            LicenseKey = "JB-2026-KEY",
            TotalSeats = 2,
            ExpirationDate = DateTime.UtcNow.AddYears(1)
        });

        var result = await SendAsync(new AssignLicenseCommand { LicenseId = licenseId });

        result.ShouldBeTrue();
        var item = await FindAsync<SoftwareLicense>(licenseId);
        item.ShouldNotBeNull();
        item!.AllocatedSeats.ShouldBe(1);
    }

    [Test]
    public async Task ShouldThrowExceptionWhenNoSeatsAvailable()
    {
        await RunAsDefaultUserAsync();

        var licenseId = await SendAsync(new CreateSoftwareLicenseCommand
        {
            Name = "Single User License",
            LicenseKey = "SINGLE-KEY",
            TotalSeats = 1,
            ExpirationDate = DateTime.UtcNow.AddYears(1)
        });

        await SendAsync(new AssignLicenseCommand { LicenseId = licenseId });

        await Should.ThrowAsync<InvalidOperationException>(() =>
            SendAsync(new AssignLicenseCommand { LicenseId = licenseId }));
    }
}
