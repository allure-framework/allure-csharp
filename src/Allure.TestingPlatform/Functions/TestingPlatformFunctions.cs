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
    /// Gets the informational version of the assembly containing the specified type,
    /// without build metadata.
    /// </summary>
    /// <param name="anchorType">A type from the assembly whose version is requested.</param>
    /// <returns>The assembly informational version without build metadata.</returns>
    /// <exception cref="InvalidOperationException">
    /// The assembly does not define an informational version.
    /// </exception>
    public static string GetPackageVersion(Type anchorType) =>
        anchorType.Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion
            ?.Split('+')
            .First()
            ?? throw new InvalidOperationException("Could not read the package version.");
}
