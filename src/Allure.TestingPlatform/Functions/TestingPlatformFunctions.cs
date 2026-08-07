using System;
using System.Linq;
using System.Reflection;

namespace Allure.TestingPlatform.Functions;

/// <summary>
/// Provides helpers for Microsoft Testing Platform integration.
/// </summary>
public static class TestingPlatformFunctions
{
    /// <summary>
    /// Gets the current Allure.TestingPlatform package version.
    /// </summary>
    public static string GetPackageVersion(Type anchorType) =>
        anchorType.Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion
            ?.Split('+')
            .First()
            ?? throw new InvalidOperationException("Could not read the package version.");
}
