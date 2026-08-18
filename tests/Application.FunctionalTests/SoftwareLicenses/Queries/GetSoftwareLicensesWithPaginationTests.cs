using EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands;
using EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands.CreateSoftwareLicenseCommand;
using EnterpriseLicenseSystem.Application.SoftwareLicenses.Queries.GetSoftwareLicensesWithPagination;

namespace EnterpriseLicenseSystem.Application.FunctionalTests.SoftwareLicenses.Queries;

using static Testing;

public class GetSoftwareLicensesWithPaginationTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnPaginatedLicenses()
    {
        await RunAsDefaultUserAsync();

        await SendAsync(new CreateSoftwareLicenseCommand
        {
            Name = "License A",
            LicenseKey = "KEY-A",
            TotalSeats = 10,
            ExpirationDate = DateTime.UtcNow.AddYears(1)
        });

        await SendAsync(new CreateSoftwareLicenseCommand
        {
            Name = "License B",
            LicenseKey = "KEY-B",
            TotalSeats = 5,
            ExpirationDate = DateTime.UtcNow.AddYears(1)
        });

        var query = new GetSoftwareLicensesWithPaginationQuery
        {
            PageNumber = 1,
            PageSize = 1
        };

        var result = await SendAsync(query);

        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(1);
        result.TotalCount.ShouldBe(2);
        result.TotalPages.ShouldBe(2);
        result.HasNextPage.ShouldBeTrue();
    }
}
