using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.Tests.Parameters;

class BrokenDataSourceTests
{
    [Test]
    public async Task TheoryWithThrowingMemberDataIsRecordedAsBroken()
    {
        var results = await AllureSampleRunner.RunAsync2(
            AllureSampleRegistry.TheoryWithThrowingMemberData
        );

        await Assert.That(results).HasSingleTestResult()
            .With.Status(AllureStatus.Broken);
    }
}
