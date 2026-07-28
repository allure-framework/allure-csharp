using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Display;

public class SetDescriptionHtmlTests
{
    [Test]
    public async Task SetDescriptionHtmlUpdatesCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.SetDescriptionHtml("<p>description</p>");
        });

        await Assert.That(test.DescriptionHtml)
            .IsEqualTo("<p>description</p>");
    }

    [Test]
    public async Task SetDescriptionHtmlAsyncUpdatesCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.SetDescriptionHtmlAsync(
                "<p>description</p>",
                CancellationToken.None
            );
        });

        await Assert.That(test.DescriptionHtml)
            .IsEqualTo("<p>description</p>");
    }

    [Test]
    public async Task SetDescriptionHtmlThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(
            () => environment.Run(
                _ => AllureApi.SetDescriptionHtml("<p>description</p>")
            )
        ).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task SetDescriptionHtmlAsyncThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(
            () => environment.RunAsync(
                _ => AllureApi.SetDescriptionHtmlAsync(
                    "<p>description</p>",
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
