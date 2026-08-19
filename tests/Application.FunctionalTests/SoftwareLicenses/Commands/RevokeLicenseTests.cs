using EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands.AssignLicenseCommand;
using EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands.CreateSoftwareLicenseCommand;
using EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands.RevokeLicenseCommand;
using EnterpriseLicenseSystem.Domain.Entities;

namespace EnterpriseLicenseSystem.Application.FunctionalTests.SoftwareLicenses.Commands;

using static Testing;

public class RevokeLicenseTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRevokeSeatSuccessfully()
    {
        var userId = await RunAsDefaultUserAsync();

        var licenseId = await SendAsync(new CreateSoftwareLicenseCommand
        {
            Name = "JetBrains ReSharper",
            LicenseKey = "JB-REVOKE-KEY",
            TotalSeats = 2,
            ExpirationDate = DateTime.UtcNow.AddYears(1)
        });

        await SendAsync(new AssignLicenseCommand { LicenseId = licenseId, UserId = userId });

        var result = await SendAsync(new RevokeLicenseCommand(licenseId, userId));

        result.Succeeded.ShouldBeTrue();

        var item = await FindAsync<SoftwareLicense>(licenseId);
        item.ShouldNotBeNull();
        item!.AllocatedSeats.ShouldBe(0);
    }

    [Test]
    public async Task ShouldFailWhenLicenseNotAssignedToUser()
    {
        var userId = await RunAsDefaultUserAsync();

        var licenseId = await SendAsync(new CreateSoftwareLicenseCommand
        {
            Name = "Office 365",
            LicenseKey = "O365-REVOKE-KEY",
            TotalSeats = 5,
            ExpirationDate = DateTime.UtcNow.AddYears(1)
        });

        var result = await SendAsync(new RevokeLicenseCommand(licenseId, userId));

        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldContain("This license is not currently assigned to that user.");
    }

    [Test]
    public async Task ShouldFailWhenLicenseDoesNotExist()
    {
        var userId = await RunAsDefaultUserAsync();

        var result = await SendAsync(new RevokeLicenseCommand(99, userId));

        result.Succeeded.ShouldBeFalse();
    }
}
