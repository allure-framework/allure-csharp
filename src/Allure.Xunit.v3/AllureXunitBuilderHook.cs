using Microsoft.Testing.Platform.Builder;

namespace Allure.Xunit;

public static class AllureXunitBuilderHook
{
    public static void AddExtensions(ITestApplicationBuilder builder, string[] _)
    {
        builder.AddAllureXunit();
    }
}