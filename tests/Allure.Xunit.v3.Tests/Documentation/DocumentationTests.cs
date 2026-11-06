using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.Documentation;

class DocumentationTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.DocumentationApi, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(3);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckDescriptionAttributesWork()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Documentation.DocumentationApi.AttributeTestClass.TestMethod"
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
    public async Task CheckPreconditionAttributesWork()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Documentation.DocumentationApi.AttributeTestClass.TestMethod"
        )
            .With.Preconditions(
                """
                Interface preconditions

                Base class preconditions

                Test class preconditions

                Test method preconditions
                """)
            .With.PreconditionsHtml(
                "<p>Interface preconditions HTML</p>"
                    + "<p>Base class preconditions HTML</p>"
                    + "<p>Test class preconditions HTML</p>"
                    + "<p>Test method preconditions HTML</p>");
    }

    [Test]
    public async Task CheckExpectedResultAttributesWork()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Documentation.DocumentationApi.AttributeTestClass.TestMethod"
        )
            .With.ExpectedResults(
                """
                Interface expected results

                Base class expected results

                Test class expected results

                Test method expected results
                """)
            .With.ExpectedResultsHtml(
                "<p>Interface expected results HTML</p>"
                    + "<p>Base class expected results HTML</p>"
                    + "<p>Test class expected results HTML</p>"
                    + "<p>Test method expected results HTML</p>");
    }

    [Test]
    public async Task CheckSyncDescriptionApiCallsFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Documentation.DocumentationApi.SyncCallFromMethod.TestMethod"
        )
            .With.Description("Test description")
            .With.DescriptionHtml("<p>Test HTML</p>");
    }

    [Test]
    public async Task CheckSyncPreconditionApiCallsFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Documentation.DocumentationApi.SyncCallFromMethod.TestMethod"
        )
            .With.Preconditions("Test preconditions")
            .With.PreconditionsHtml("<p>Test preconditions HTML</p>");
    }

    [Test]
    public async Task CheckSyncExpectedResultApiCallsFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Documentation.DocumentationApi.SyncCallFromMethod.TestMethod"
        )
            .With.ExpectedResults("Test expected results")
            .With.ExpectedResultsHtml("<p>Test expected results HTML</p>");
    }

    [Test]
    public async Task CheckAsyncDescriptionApiCallsFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Documentation.DocumentationApi.AsyncCallFromMethod.TestMethod"
        )
            .With.Description("Test description")
            .With.DescriptionHtml("<p>Test HTML</p>");
    }

    [Test]
    public async Task CheckAsyncPreconditionApiCallsFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Documentation.DocumentationApi.AsyncCallFromMethod.TestMethod"
        )
            .With.Preconditions("Test preconditions")
            .With.PreconditionsHtml("<p>Test preconditions HTML</p>");
    }

    [Test]
    public async Task CheckAsyncExpectedResultApiCallsFromMethod()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Documentation.DocumentationApi.AsyncCallFromMethod.TestMethod"
        )
            .With.ExpectedResults("Test expected results")
            .With.ExpectedResultsHtml("<p>Test expected results HTML</p>");
    }
}
