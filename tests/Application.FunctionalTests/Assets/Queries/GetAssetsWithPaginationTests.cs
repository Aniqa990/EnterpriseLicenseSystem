using EnterpriseLicenseSystem.Application.Assets.Commands.CreateAsset;
using EnterpriseLicenseSystem.Application.Assets.Queries.GetAssetsWithPagination;

namespace EnterpriseLicenseSystem.Application.FunctionalTests.Assets.Queries;

using static Testing;

public class GetAssetsWithPaginationTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnPaginatedAssets()
    {
        await RunAsDefaultUserAsync();

        await SendAsync(new CreateAssetCommand { Model = "Asset A", SerialNumber = "PG-A" });
        await SendAsync(new CreateAssetCommand { Model = "Asset B", SerialNumber = "PG-B" });
        await SendAsync(new CreateAssetCommand { Model = "Asset C", SerialNumber = "PG-C" });

        var query = new GetAssetsWithPaginationQuery
        {
            PageNumber = 1,
            PageSize = 2
        };

        var result = await SendAsync(query);

        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(2);
        result.TotalCount.ShouldBe(3);
        result.TotalPages.ShouldBe(2);
        result.HasNextPage.ShouldBeTrue();
        result.HasPreviousPage.ShouldBeFalse();
    }
}
