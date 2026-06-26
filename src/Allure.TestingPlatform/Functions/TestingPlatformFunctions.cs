using System;
using System.Linq;
using System.Reflection;

namespace Allure.TestingPlatform.Functions;

public static class TestingPlatformFunctions
{
    public static string CurrentPackageVersion { get; } =
        Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion
            ?.Split('+')
            .First()
            ?? throw new InvalidOperationException("Couldn't read the package version");
}