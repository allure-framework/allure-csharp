using Allure.Testing;

namespace Allure.Xunit.v3.Tests.AllureIds;

class AllureIdTests
{
    [Test]
    public async Task CheckAllureIdAttributeWorks(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AllureIdAttributeOnMethod, token);

        await Assert.That(results).HasSingleTestResult()
            .That.HasSingleLabel("ALLURE_ID")
            .With.Value("1001");
    }
}
