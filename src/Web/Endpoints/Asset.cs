using EnterpriseLicenseSystem.Application.Assets.Commands.AssignAsset;
using EnterpriseLicenseSystem.Application.Assets.Commands.CreateAsset;
using EnterpriseLicenseSystem.Application.Assets.Commands.DeleteAsset;
using EnterpriseLicenseSystem.Application.Assets.Commands.UnassignAsset;
using EnterpriseLicenseSystem.Application.Assets.Commands.UpdateAsset;
using EnterpriseLicenseSystem.Application.Assets.Queries;
using EnterpriseLicenseSystem.Application.Assets.Queries.GetAssetsWithPagination;
using EnterpriseLicenseSystem.Application.Common.Models;
using EnterpriseLicenseSystem.Domain.Constants;
using EnterpriseLicenseSystem.Web.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EnterpriseLicenseSystem.Web.Endpoints;

public class Assets : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder app)
    {
        app.MapGet(GetAssetsWithPagination)
           .RequireAuthorization();

        app.MapPost(CreateAsset)
           .RequireAuthorization(policy => policy.RequireRole(Roles.Administrator, Roles.LicenseManager))
           .RequireRateLimiting(RateLimitPolicies.WriteOperations);

        app.MapPut(UpdateAsset, "{id}")
           .RequireAuthorization(policy => policy.RequireRole(Roles.Administrator, Roles.LicenseManager))
           .RequireRateLimiting(RateLimitPolicies.WriteOperations);

        app.MapPut("/{id}/assign", AssignAsset)
           .RequireAuthorization(policy => policy.RequireRole(Roles.Administrator, Roles.LicenseManager))
           .RequireRateLimiting(RateLimitPolicies.WriteOperations);

        app.MapPut("/{id}/unassign", UnassignAsset)
           .RequireAuthorization(policy => policy.RequireRole(Roles.Administrator, Roles.LicenseManager))
           .RequireRateLimiting(RateLimitPolicies.WriteOperations);

        app.MapDelete(DeleteAsset, "{id}")
           .RequireAuthorization(policy => policy.RequireRole(Roles.Administrator))
           .RequireRateLimiting(RateLimitPolicies.WriteOperations);
    }

    public Task<PaginatedList<AssetBriefDto>> GetAssetsWithPagination(
        ISender sender,
        [AsParameters] GetAssetsWithPaginationQuery query)
    {
        return sender.Send(query);
    }

    public Task<int> CreateAsset(ISender sender, CreateAssetCommand command)
    {
        return sender.Send(command);
    }

    public async Task<Results<NoContent, BadRequest>> UpdateAsset(ISender sender, int id, UpdateAssetCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();

        await sender.Send(command);

        return TypedResults.NoContent();
    }

    public async Task<IResult> AssignAsset(ISender sender, int id, string userId)
    {
        var result = await sender.Send(new AssignAssetCommand(id, userId));

        return result.Succeeded
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { errors = result.Errors });
    }

    public async Task<IResult> UnassignAsset(ISender sender, int id)
    {
        var result = await sender.Send(new UnassignAssetCommand(id));

        return result.Succeeded
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { errors = result.Errors });
    }

    public async Task<NoContent> DeleteAsset(ISender sender, int id)
    {
        await sender.Send(new DeleteAssetCommand(id));

        return TypedResults.NoContent();
    }
}
