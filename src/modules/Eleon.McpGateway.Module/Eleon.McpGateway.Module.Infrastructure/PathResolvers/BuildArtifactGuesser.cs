using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

namespace Eleon.McpGateway.Module.Infrastructure.PathResolvers;

internal static class BuildArtifactGuesser
{
    public static string GetCurrentConfiguration()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

        // Walk up the directory tree until we find a conventional configuration folder (Debug/Release).
        for (var directory = current; directory is not null; directory = directory.Parent)
        {
            if (IsConfigurationFolder(directory.Name))
            {
                return directory.Name;
            }
        }

        // Fallback for unusual layouts.
        return "Debug";
    }

    public static string GetCurrentTargetFramework()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

        // Try to spot a TFM-looking folder (e.g., net9.0) while walking up the tree.
        for (var directory = current; directory is not null; directory = directory.Parent)
        {
            if (LooksLikeTargetFramework(directory.Name))
            {
                return directory.Name;
            }
        }

        // Fallback to the app's target framework attribute if available.
        var frameworkName = Assembly.GetEntryAssembly()?
            .GetCustomAttribute<TargetFrameworkAttribute>()?
            .FrameworkName;

        if (!string.IsNullOrWhiteSpace(frameworkName))
        {
            // FrameworkName is like ".NETCoreApp,Version=v8.0" -> convert to net9.0
            var version = new Version(frameworkName.Split("Version=v").LastOrDefault() ?? "8.0");
            return $"net{version.Major}.{version.Minor}";
        }

        return "net9.0";
    }

    private static bool IsConfigurationFolder(string name) =>
        name.Equals("Debug", StringComparison.OrdinalIgnoreCase) ||
        name.Equals("Release", StringComparison.OrdinalIgnoreCase);

    private static bool LooksLikeTargetFramework(string name) =>
        name.StartsWith("net", StringComparison.OrdinalIgnoreCase) &&
        name.Length >= 4; // e.g., net9.0, net7.0, netstandard2.1
}

