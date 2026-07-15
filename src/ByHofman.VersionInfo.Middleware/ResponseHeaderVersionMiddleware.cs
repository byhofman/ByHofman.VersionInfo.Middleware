using Microsoft.AspNetCore.Http;

namespace ByHofman.VersionInfo.Middleware;

public sealed class ResponseHeaderVersionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Version"] = VersionHelper.AssemblyVersion();
                return Task.CompletedTask;
            });
        }

        await next(context);
    }
}
