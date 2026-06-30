using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Allure.Net.Commons;
using Microsoft.Testing.Platform.Services;

namespace Allure.TestingPlatform.Functions;

/// <summary>
/// Provides helpers for Microsoft Testing Platform integration.
/// </summary>
public static class TestingPlatformFunctions
{
    /// <summary>
    /// Gets the current Allure.TestingPlatform package version.
    /// </summary>
    public static string CurrentPackageVersion { get; } =
        Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion
            ?.Split('+')
            .First()
            ?? throw new InvalidOperationException("Could not read the package version.");

    /// <summary>
    /// Tries to resolve the default Allure results directory from Microsoft Testing Platform
    /// configuration.
    /// </summary>
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
