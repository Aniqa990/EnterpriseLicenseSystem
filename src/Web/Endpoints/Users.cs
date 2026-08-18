using EnterpriseLicenseSystem.Infrastructure.Identity;

namespace EnterpriseLicenseSystem.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder app)
    {
        app.MapIdentityApi<ApplicationUser>();
    }
}
