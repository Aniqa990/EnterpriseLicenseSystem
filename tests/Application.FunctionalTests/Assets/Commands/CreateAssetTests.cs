using EnterpriseLicenseSystem.Application.Assets.Commands.CreateAsset;
using EnterpriseLicenseSystem.Application.Common.Exceptions;
using EnterpriseLicenseSystem.Domain.Entities;

namespace EnterpriseLicenseSystem.Application.FunctionalTests.Assets.Commands;

using static Testing;

public class CreateAssetTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateAssetCommand();

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateAsset()
    {
        var userId = await RunAsDefaultUserAsync();

        var command = new CreateAssetCommand
        {
            Model = "Dell Latitude 7440",
            SerialNumber = "SN-0001"
        };

        var assetId = await SendAsync(command);

        var asset = await FindAsync<Asset>(assetId);

        asset.ShouldNotBeNull();
        asset!.Model.ShouldBe(command.Model);
        asset.SerialNumber.ShouldBe(command.SerialNumber);
        asset.CreatedBy.ShouldBe(userId);
    }
}
