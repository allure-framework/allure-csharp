using Allure.Testing;

namespace Allure.Xunit.v3.Tests.Descriptions;

class DescriptionTests
{
    [Test]
    public async Task CheckDescriptionAttributesWork(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.DescriptionAttributes, token);

        await Assert.That(results).HasSingleTestResult()
            .With.Description(
                """
                Interface description

                Base class description

                Test class description

                Test method description
                """)
            .With.DescriptionHtml(
                "<p>Interface HTML</p>"
                    + "<p>Base class HTML</p>"
                    + "<p>Test class HTML</p>"
                    + "<p>Test method HTML</p>");
    }
}
