# Usage guide

## Requirements

- .NET 8 or later (ASP.NET Core)
- Authentication middleware configured in the pipeline

## 1. Register the middleware

Call `AddVersionInfoMiddleware` in the service registration section of `Program.cs`:

```csharp
builder.Services.AddVersionInfoMiddleware();
```

## 2. Add it to the pipeline

Call `UseVersionInfoMiddleware` **after** `UseAuthentication` and `UseAuthorization`. Placing it before the auth middleware means `HttpContext.User` is not yet populated and the header will never be written.

```csharp
app.UseAuthentication();
app.UseAuthorization();
app.UseVersionInfoMiddleware(); // must come after auth
```

## 3. Verify

Send an authenticated request to any endpoint. The response will include:

```
X-Version: 1.2.3
```

Anonymous requests receive no `X-Version` header.

## Advanced: reading the version in code

`VersionHelper.AssemblyVersion()` returns the same string without going through HTTP. Useful for health checks or log enrichment:

```csharp
using ByHofman.VersionInfo.Middleware;

var version = VersionHelper.AssemblyVersion();
```

By default it reads from `Assembly.GetEntryAssembly()` (the host application). You can pass a specific assembly:

```csharp
var version = VersionHelper.AssemblyVersion(typeof(MyClass).Assembly);
```
