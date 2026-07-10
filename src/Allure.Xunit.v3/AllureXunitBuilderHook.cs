using Microsoft.Testing.Platform.Builder;

#pragma warning disable IDE0060

namespace Allure.Xunit;

/// <summary>
/// Provides the Microsoft Testing Platform builder hook to enable self-registration of Allure.Xunit.v3.
/// </summary>
public static class AllureXunitBuilderHook
{
    /// <summary>
    /// Registers Allure.Xunit.v3 with the Microsoft Testing Platform test application builder.
    /// </summary>
    /// <param name="builder">The test application builder to configure.</param>
    /// <param name="args">The command-line arguments passed to the test application.</param>
    public static void AddExtensions(ITestApplicationBuilder builder, string[] args)
    {
        builder.AddAllureXunit();
    }
}
