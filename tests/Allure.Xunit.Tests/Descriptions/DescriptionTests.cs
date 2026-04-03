using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.Xunit.Tests.Descriptions;

class DescriptionTests
{
    [Test]
    public async Task CheckDescriptionAndDescriptionHtmlCanBeAdded()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AddDescriptionFromTestHtmlFromDispose);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        await Assert.That((string)results.TestResults[0]["description"]).IsEqualTo("Lorem Ipsum");
        await Assert.That((string)results.TestResults[0]["descriptionHtml"]).IsEqualTo("Dolor Sit Amet");
    }

    [Test]
    public async Task CheckDescriptionHtmlAndDescriptionCanBeAdded()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AddDescriptionFromDisposeHtmlFromTest);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        await Assert.That((string)results.TestResults[0]["descriptionHtml"]).IsEqualTo("Lorem Ipsum");
        await Assert.That((string)results.TestResults[0]["description"]).IsEqualTo("Dolor Sit Amet");
    }

    [Test]
    public async Task DescriptionAttributeShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.DescriptionAttributes);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        await Assert.That((string)results.TestResults[0]["description"]).IsEqualTo(
            """
            Lorem Ipsum

            Consectetur Adipiscing Elit

            Tempor Incididunt

            Et Dolore
            """
        );
        await Assert.That((string)results.TestResults[0]["descriptionHtml"]).IsEqualTo(
            "<p>Dolor Sit Amet</p>"
                + "<p>Sed Do Eiusmod</p>"
                + "<p>Ut Labore</p>"
                + "<p>Magna Aliqua</p>"
        );
    }

    [Test]
    public async Task LegacyDescriptionAttributeShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LegacyDescriptionAttribute);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        await Assert.That((string)results.TestResults[0]["description"]).IsEqualTo("Lorem Ipsum");
    }
}
