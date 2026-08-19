using EnterpriseLicenseSystem.Application.Assets.Commands.AssignAsset;
using EnterpriseLicenseSystem.Application.Assets.Commands.CreateAsset;
using EnterpriseLicenseSystem.Application.Assets.Commands.DeleteAsset;
using EnterpriseLicenseSystem.Application.Assets.Commands.UnassignAsset;
using EnterpriseLicenseSystem.Domain.Entities;

namespace EnterpriseLicenseSystem.Application.FunctionalTests.Assets.Commands;

using static Testing;

public class AssignAssetTests : BaseTestFixture
{
    [Test]
    public async Task ShouldAssignAndUnassignSuccessfully()
    {
        var userId = await RunAsDefaultUserAsync();

        var assetId = await SendAsync(new CreateAssetCommand
        {
            Model = "MacBook Pro 16\"",
            SerialNumber = "SN-ASSIGN-0001"
        });

        var assignResult = await SendAsync(new AssignAssetCommand(assetId, userId));
        assignResult.Succeeded.ShouldBeTrue();

        var asset = await FindAsync<Asset>(assetId);
        asset.ShouldNotBeNull();
        asset!.AssignedToUserId.ShouldBe(userId);

        var unassignResult = await SendAsync(new UnassignAssetCommand(assetId));
        unassignResult.Succeeded.ShouldBeTrue();

        asset = await FindAsync<Asset>(assetId);
        asset!.AssignedToUserId.ShouldBeNull();
    }

    [Test]
    public async Task ShouldFailWhenAssigningTwiceToSameUser()
    {
        var userId = await RunAsDefaultUserAsync();

        var assetId = await SendAsync(new CreateAssetCommand
        {
            Model = "ThinkPad X1",
            SerialNumber = "SN-ASSIGN-0002"
        });

        await SendAsync(new AssignAssetCommand(assetId, userId));

        var result = await SendAsync(new AssignAssetCommand(assetId, userId));

        result.Succeeded.ShouldBeFalse();
    }

    [Test]
    public async Task ShouldSoftDeleteAsset()
    {
        await RunAsDefaultUserAsync();

        var assetId = await SendAsync(new CreateAssetCommand
        {
            Model = "iPad Air",
            SerialNumber = "SN-DELETE-0001"
        });

        await SendAsync(new DeleteAssetCommand(assetId));

        // FindAsync bypasses global query filters, so the row is still readable here, its soft deleted
        var asset = await FindAsync<Asset>(assetId);
        asset.ShouldNotBeNull();
        asset!.IsDeleted.ShouldBeTrue();
    }
}
