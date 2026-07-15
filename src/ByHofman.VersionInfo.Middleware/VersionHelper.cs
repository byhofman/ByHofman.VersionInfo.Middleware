using System.Diagnostics;
using System.Reflection;

namespace ByHofman.VersionInfo.Middleware;

public static class VersionHelper
{
    private static string? _versionNumber;

    public static string AssemblyVersion(Assembly? assembly = null)
    {
        if (_versionNumber != null)
        {
            return _versionNumber;
        }

        assembly ??= Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
        _versionNumber = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion ?? "Unknown";

        return _versionNumber;
    }
}
