using System.Collections.Immutable;
using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.Lifecycle;

class FailExceptionTests
{
    [Test]
    public async Task CheckPassingFactIsRecordedAsPassed(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(
            AllureSampleRegistry.SingleBroken,
            new()
            {
                AllureConfiguration = new
                {
                    allure = new
                    {
                        failExceptions = ImmutableArray.Create("System.InvalidOperationException"),
                    }
                },
            },
            token
        );

        await Assert.That(results).HasSingleTestResult().With.Status(AllureStatus.Failed);
    }
}
