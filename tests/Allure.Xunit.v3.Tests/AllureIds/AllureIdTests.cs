using Allure.Testing;

namespace Allure.Xunit.v3.Tests.AllureIds;

class AllureIdTests
{
    [Test]
    public async Task CheckAllureIdAttributeWorks()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AllureIdAttributeOnMethod);

        await Assert.That(results).HasSingleTestResult()
            .That.HasSingleLabel("ALLURE_ID")
            .With.Value("1001");
    }
}
