using System;
using System.Linq;
using System.Reflection;

namespace Allure.TestingPlatform.Functions;

public static class ExtensionFunctions
{
    public static string? CurrentPackageVersion { get; } =
        Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion
            ?.Split('+')
            .First();

    public static string GetCurrentPackageVersion(string fallback) =>
        CurrentPackageVersion ?? fallback;

    public static string GetCurrentPackageVersion() =>
        CurrentPackageVersion
            ?? throw new InvalidOperationException("Couldn't read the package version");
}
