using Allure.Testing;

namespace Allure.Xunit.v3.Tests.Lifecycle;

class WatchdogTests
{
    [Test]
    public async Task ShouldRecordGlobalErrorWhenTestProcessCrashes(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(
            AllureSampleRegistry.CrashingProcess,
            token
        );

        await Assert.That(results)
            .HasSingleGlobals()
            .With.SingleError((error) => error.HasMessage((message) =>
                message
                    .Contains("Test host application process")
                    .And.Contains("(PID=")
                    .And.Contains("has crashed")
                    .And.Contains("Exit code:")));
    }
}
