using EnterpriseLicenseSystem.Application.Common.Exceptions;
using EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands;
using EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands.CreateSoftwareLicenseCommand;
using EnterpriseLicenseSystem.Domain.Entities;

namespace EnterpriseLicenseSystem.Application.FunctionalTests.SoftwareLicenses.Commands;

using static Testing;

public class CreateSoftwareLicenseTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateSoftwareLicenseCommand();

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateSoftwareLicense()
    {
        var userId = await RunAsDefaultUserAsync();

        var command = new CreateSoftwareLicenseCommand
        {
            Name = "Visual Studio Enterprise",
            LicenseKey = "VS-2026-KEY",
            TotalSeats = 25,
            ExpirationDate = DateTime.UtcNow.AddYears(1)
        };

        var licenseId = await SendAsync(command);

        var item = await FindAsync<SoftwareLicense>(licenseId);

        item.ShouldNotBeNull();
        item!.Name.ShouldBe(command.Name);
        item.LicenseKey.ShouldBe(command.LicenseKey);
        item.TotalSeats.ShouldBe(command.TotalSeats);
        item.AllocatedSeats.ShouldBe(0);
        item.CreatedBy.ShouldBe(userId);
        item.Created.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
