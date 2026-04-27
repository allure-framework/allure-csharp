using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.Xunit.v3.Tests;

class SmokeTests
{
    [Test]
    public async Task SampleRunProducesSingleAllureResult()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.SetAllureIdFromTest);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
    }
}
