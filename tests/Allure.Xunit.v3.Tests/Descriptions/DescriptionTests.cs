using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.Descriptions;

class DescriptionTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.DescriptionApi, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(3);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckDescriptionAttributesWork()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Descriptions.DescriptionApi.AttributeTestClass.TestMethod"
        )
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

    [Test]
    public async Task CheckSyncDescriptionApiCallsFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Descriptions.DescriptionApi.SyncCallFromMethod.TestMethod"
        )
            .With.Description("Test description")
            .With.DescriptionHtml("<p>Test HTML</p>");
    }

    [Test]
    public async Task CheckAsyncDescriptionApiCallsFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Descriptions.DescriptionApi.AsyncCallFromMethod.TestMethod"
        )
            .With.Description("Test description")
            .With.DescriptionHtml("<p>Test HTML</p>");
    }
}
