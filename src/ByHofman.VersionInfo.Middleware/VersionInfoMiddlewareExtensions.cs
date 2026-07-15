using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ByHofman.VersionInfo.Middleware;

public static class VersionInfoMiddlewareExtensions
{
    public static IServiceCollection AddVersionInfoMiddleware(this IServiceCollection services)
    {
        services.AddTransient<ResponseHeaderVersionMiddleware>();
        return services;
    }

    public static IApplicationBuilder UseVersionInfoMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ResponseHeaderVersionMiddleware>();
    }
}
