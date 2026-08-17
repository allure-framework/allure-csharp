using System;
using System.Linq;
using System.Reflection;

namespace Allure.TestingPlatform.Functions;

/// <summary>
/// Resolves package versions from assembly metadata.
/// </summary>
public static class PackageVersions
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
    public static string For(Type anchorType) =>
        anchorType.Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion
            ?.Split('+')
            .First()
            ?? throw new InvalidOperationException("Could not read the package version.");
}
