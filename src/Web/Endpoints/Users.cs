using EnterpriseLicenseSystem.Application.Common.Interfaces;
using EnterpriseLicenseSystem.Domain.Constants;
using EnterpriseLicenseSystem.Infrastructure.Identity;

namespace EnterpriseLicenseSystem.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder app)
    {
        app.MapIdentityApi<ApplicationUser>();
        app.MapPost("/assign-role", AssignRole)
           .RequireAuthorization(policy => policy.RequireRole(Roles.Administrator));
    }

    public async Task<IResult> AssignRole(IIdentityService identityService, AssignRoleRequest request)
    {
        var validRoles = new[] { Roles.Administrator, Roles.LicenseManager, Roles.Employee };

        if (!validRoles.Contains(request.Role))
        {
            return Results.BadRequest(new { errors = new[] { $"Invalid role '{request.Role}'." } });
        }

        var result = await identityService.AddToRolesAsync(request.UserId, new[] { request.Role });

        return result.Succeeded
            ? Results.Ok(new { message = $"Role '{request.Role}' assigned successfully." })
            : Results.BadRequest(new { errors = result.Errors });
    }
}

public record AssignRoleRequest(string UserId, string Role);
