using Allure.Testing;

namespace Allure.Xunit.Tests.Descriptions;

class DescriptionTests
{
    [Test]
    public async Task CheckRuntimeApiDescriptionFromTestAndDescriptionHtmlFromDispose()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AddDescriptionFromTestHtmlFromDispose);

        await Assert.That(results).HasSingleTestResult()
            .With.Description("Lorem Ipsum")
            .With.DescriptionHtml("Dolor Sit Amet");
    }

    [Test]
    public async Task CheckRuntimeApiDescriptionHtmlFromTestAndDescriptionFromDispose()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AddDescriptionFromDisposeHtmlFromTest);

        await Assert.That(results).HasSingleTestResult()
            .With.DescriptionHtml("Lorem Ipsum")
            .With.Description("Dolor Sit Amet");
    }

    [Test]
    public async Task DescriptionAttributeShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.DescriptionAttributes);

        await Assert.That(results).HasSingleTestResult()
            .With.Description(
                """
                Lorem Ipsum

                Consectetur Adipiscing Elit

                Tempor Incididunt

                Et Dolore
                """)
            .With.DescriptionHtml(
                "<p>Dolor Sit Amet</p>"
                    + "<p>Sed Do Eiusmod</p>"
                    + "<p>Ut Labore</p>"
                    + "<p>Magna Aliqua</p>");
    }

    [Test]
    public async Task LegacyDescriptionAttributeShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LegacyDescriptionAttribute);

        await Assert.That(results).HasSingleTestResult()
            .With.Description("Lorem Ipsum");
    }
}
