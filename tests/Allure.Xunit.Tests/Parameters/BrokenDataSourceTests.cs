using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.Xunit.Tests.Parameters;

class BrokenDataSourceTests
{
    [Test]
    public async Task TheoryWithThrowingMemberDataIsRecordedAsBroken()
    {
        var results = await AllureSampleRunner.RunAsync(
            AllureSampleRegistry.TheoryWithThrowingMemberData
        );

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        await Assert.That((string)results.TestResults[0]["status"]).IsEqualTo("broken");
    }
}
