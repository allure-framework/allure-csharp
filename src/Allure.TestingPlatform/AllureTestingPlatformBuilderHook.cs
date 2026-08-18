using Microsoft.Testing.Platform.Builder;

namespace Allure.TestingPlatform;

/// <summary>
/// Provides the self-registration entry point used by Microsoft.Testing.Platform.MSBuild.
/// </summary>
public static class AllureTestingPlatformBuilderHook
{
    /// <summary>
    /// Implements the Microsoft.Testing.Platform.MSBuild self-registration contract and registers
    /// Allure.TestingPlatform in standalone mode with default options.
    /// </summary>
    /// <param name="builder">The test application builder.</param>
    /// <param name="_">The command-line arguments supplied during registration.</param>
    public static void AddExtensions(ITestApplicationBuilder builder, string[] _)
    {
        builder.AddAllure();
    }
}
