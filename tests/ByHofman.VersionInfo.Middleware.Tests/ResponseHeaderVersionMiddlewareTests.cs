using System.Security.Claims;
using System.Text.Encodings.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Xunit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ByHofman.VersionInfo.Middleware.Tests;

public class ResponseHeaderVersionMiddlewareTests : IAsyncLifetime
{
    private IHost _host = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _host = await new HostBuilder()
            .ConfigureWebHost(web =>
            {
                web.UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddVersionInfoMiddleware();
                        services.AddAuthentication("Fake")
                            .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Fake", _ => { });
                    })
                    .Configure(app =>
                    {
                        app.UseAuthentication();
                        app.UseVersionInfoMiddleware();
                        app.Run(ctx => ctx.Response.WriteAsync("ok"));
                    });
            })
            .StartAsync();

        _client = _host.GetTestClient();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }

    [Fact]
    public async Task InvokeAsync_AuthenticatedRequest_AddsXVersionHeader()
    {
        _client.DefaultRequestHeaders.Add("X-Authenticated", "true");

        var response = await _client.GetAsync("/");

        response.Headers.Contains("X-Version").Should().BeTrue();
        response.Headers.GetValues("X-Version").First().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task InvokeAsync_AnonymousRequest_DoesNotAddXVersionHeader()
    {
        var response = await _client.GetAsync("/");

        response.Headers.Contains("X-Version").Should().BeFalse();
    }

    [Fact]
    public async Task InvokeAsync_AuthenticatedRequest_VersionMatchesVersionHelper()
    {
        _client.DefaultRequestHeaders.Add("X-Authenticated", "true");

        var response = await _client.GetAsync("/");

        var headerValue = response.Headers.GetValues("X-Version").First();
        headerValue.Should().Be(VersionHelper.AssemblyVersion());
    }

    [Fact]
    public async Task InvokeAsync_AnonymousRequest_StillReturnsSuccessResponse()
    {
        var response = await _client.GetAsync("/");

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_AuthenticatedRequest_StillReturnsSuccessResponse()
    {
        _client.DefaultRequestHeaders.Add("X-Authenticated", "true");

        var response = await _client.GetAsync("/");

        response.IsSuccessStatusCode.Should().BeTrue();
    }
}

internal sealed class FakeAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("X-Authenticated"))
            return Task.FromResult(AuthenticateResult.NoResult());

        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, "testuser")], Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
