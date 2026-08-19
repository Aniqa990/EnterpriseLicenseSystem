using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace EnterpriseLicenseSystem.Web.Infrastructure;

public static class RateLimitPolicies
{
    public const string WriteOperations = "write-operations";
}

public static class RateLimitingServiceCollectionExtensions
{
    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(RateLimitPolicies.WriteOperations, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));
        });

        return services;
    }
}
