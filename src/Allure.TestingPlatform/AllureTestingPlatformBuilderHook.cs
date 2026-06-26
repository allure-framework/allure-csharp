using Microsoft.Testing.Platform.Builder;

namespace Allure.TestingPlatform;

public static class AllureTestingPlatformBuilderHook
{
    public static void AddExtensions(ITestApplicationBuilder builder, string[] _)
    {
        builder.AddAllure();
    }
}
