using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Allure.Net.Commons;
using Microsoft.Testing.Platform.Services;

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

    public static bool TryGetDefaultAllureResultsPath(
        IServiceProvider serviceProvider,
        [NotNullWhen(true)] out string? allureResultsDir
    )
    {
        if (serviceProvider.GetConfiguration()["platformOptions:resultDirectory"] is { } mtpResultsDir)
        {
            allureResultsDir = Path.Combine(mtpResultsDir, AllureConstants.DEFAULT_RESULTS_FOLDER);
            return true;
        }

        allureResultsDir = null;
        return false;
    }
}