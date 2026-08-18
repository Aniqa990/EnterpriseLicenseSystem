using EnterpriseLicenseSystem.Application.Common.Models;
using EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands;
using EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands.AssignLicenseCommand;
using EnterpriseLicenseSystem.Application.SoftwareLicenses.Commands.CreateSoftwareLicenseCommand;
using EnterpriseLicenseSystem.Application.SoftwareLicenses.Queries;
using EnterpriseLicenseSystem.Application.SoftwareLicenses.Queries.GetSoftwareLicensesWithPagination;
using EnterpriseLicenseSystem.Domain.Constants;
using EnterpriseLicenseSystem.Web.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;

namespace EnterpriseLicenseSystem.Web.Endpoints;

public class SoftwareLicenses : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder app)
    {
        app.MapPost(CreateSoftwareLicense)
           .RequireAuthorization(policy => policy.RequireRole(Roles.Administrator, Roles.LicenseManager));

        app.MapPut("/{id}/assign", AssignLicense)
           .RequireAuthorization(policy => policy.RequireRole(Roles.Administrator, Roles.LicenseManager));

        app.MapGet(GetSoftwareLicensesWithPagination)
           .RequireAuthorization();
    }

    public Task<PaginatedList<SoftwareLicenseBriefDto>> GetSoftwareLicensesWithPagination(
        ISender sender,
        [AsParameters] GetSoftwareLicensesWithPaginationQuery query)
    {
        return sender.Send(query);
    }

    public Task<int> CreateSoftwareLicense(ISender sender, CreateSoftwareLicenseCommand command)
    {
        return sender.Send(command);
    }

    public Task<bool> AssignLicense(ISender sender, int id)
    {
        return sender.Send(new AssignLicenseCommand(id));
    }
}
