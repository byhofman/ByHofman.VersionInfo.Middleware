# ByHofman.VersionInfo.Middleware

ASP.NET Core middleware that adds an `X-Version` response header containing the application's assembly version. The header is only exposed to authenticated users.

## Installation

The package is published to GitHub Packages. Add the `byhofman` NuGet source once, then install:

```
dotnet nuget add source "https://nuget.pkg.github.com/byhofman/index.json" \
  --name byhofman --username <github-username> --password <github-token>

dotnet add package ByHofman.VersionInfo.Middleware
```

> GitHub Packages requires authentication even for public packages. Use a personal access token with the `read:packages` scope.

## Quick start

```csharp
// Program.cs
builder.Services.AddVersionInfoMiddleware();

// Must be placed after UseAuthentication
app.UseAuthentication();
app.UseAuthorization();
app.UseVersionInfoMiddleware();
```

Authenticated requests will receive an `X-Version` response header. Anonymous requests receive no version information.

## How versioning works

The package version is taken from the git tag at publish time. Tag a commit with a `v`-prefixed semantic version (e.g. `v1.2.0`) and push the tag to publish version `1.2.0`.

## Documentation

- [Usage guide](docs/Usage.md)
