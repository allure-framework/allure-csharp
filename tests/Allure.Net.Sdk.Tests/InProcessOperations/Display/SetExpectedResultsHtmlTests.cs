using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Display;

public class SetExpectedResultsHtmlTests
{
    [Test]
    public async Task SetExpectedResultsHtmlUpdatesCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.SetExpectedResultsHtml("<p>expected results</p>");
        });

        await Assert.That(test.ExpectedResultsHtml)
            .IsEqualTo("<p>expected results</p>");
    }

    [Test]
    public async Task SetExpectedResultsHtmlAsyncUpdatesCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.SetExpectedResultsHtmlAsync(
                "<p>expected results</p>",
                CancellationToken.None
            );
        });

        await Assert.That(test.ExpectedResultsHtml)
            .IsEqualTo("<p>expected results</p>");
    }

    [Test]
    public async Task SetExpectedResultsHtmlThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(
            () => environment.Run(
                _ => AllureApi.SetExpectedResultsHtml("<p>expected results</p>")
            )
        ).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task SetExpectedResultsHtmlAsyncThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(
            () => environment.RunAsync(
                _ => AllureApi.SetExpectedResultsHtmlAsync(
                    "<p>expected results</p>",
                    CancellationToken.None
                )
            )
        ).Throws<InvalidOperationException>();
    }

    static AllureTestResult NewTest() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "test",
    };
}
