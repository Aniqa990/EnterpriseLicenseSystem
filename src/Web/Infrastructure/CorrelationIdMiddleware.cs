namespace EnterpriseLicenseSystem.Web.Infrastructure;

// Distributed tracing: reads X-Correlation-ID from the incoming request (so an upstream
// gateway/service can propagate one), otherwise generates a new one. Echoes it back on
// the response and pushes it into the log context so every Serilog line for this request
// carries the same CorrelationId — invaluable when correlating logs across services.
public class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-ID";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var existing)
                             && !string.IsNullOrWhiteSpace(existing)
            ? existing.ToString()
            : Guid.NewGuid().ToString();

        context.Items[HeaderName] = correlationId;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}

public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        => builder.UseMiddleware<CorrelationIdMiddleware>();
}
