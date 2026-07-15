using System.Reflection;
using FluentAssertions;
using Xunit;

namespace ByHofman.VersionInfo.Middleware.Tests;

public class VersionHelperTests
{
    [Fact]
    public void AssemblyVersion_ReturnsNonNullNonEmptyString()
    {
        var version = VersionHelper.AssemblyVersion();

        version.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void AssemblyVersion_CalledMultipleTimes_ReturnsSameValue()
    {
        var first = VersionHelper.AssemblyVersion();
        var second = VersionHelper.AssemblyVersion();

        second.Should().Be(first);
    }

    [Fact]
    public void AssemblyVersion_WithExplicitAssembly_ReturnsNonNullNonEmptyString()
    {
        var version = VersionHelper.AssemblyVersion(Assembly.GetExecutingAssembly());

        version.Should().NotBeNullOrWhiteSpace();
    }
}
