using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Capabilities.TestFramework;

namespace Allure.TestingPlatform.Tests.SdkExtensibility;

static class ExtensibilityTestApplication
{
    static readonly string[] DefaultArgs =
    [
        "--no-progress",
        "--no-ansi",
        "--output",
        "Normal",
        "--show-stdout",
        "None",
        "--show-stderr",
        "None",
    ];

    public static Task<ITestApplicationBuilder> CreateBuilderAsync() =>
        TestApplication.CreateBuilderAsync(DefaultArgs);

    public static void RegisterTestFramework(ITestApplicationBuilder builder) =>
        builder.RegisterTestFramework(
            _ => new TestFrameworkCapabilities(),
            (_, _) => new TestFrameworkStub()
        );
}
