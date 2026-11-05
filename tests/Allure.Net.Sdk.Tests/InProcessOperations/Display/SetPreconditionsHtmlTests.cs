using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Display;

public class SetPreconditionsHtmlTests
{
    [Test]
    public async Task SetPreconditionsHtmlUpdatesCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.SetPreconditionsHtml("<p>preconditions</p>");
        });

        await Assert.That(test.PreconditionsHtml)
            .IsEqualTo("<p>preconditions</p>");
    }

    [Test]
    public async Task SetPreconditionsHtmlAsyncUpdatesCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.SetPreconditionsHtmlAsync(
                "<p>preconditions</p>",
                CancellationToken.None
            );
        });

        await Assert.That(test.PreconditionsHtml)
            .IsEqualTo("<p>preconditions</p>");
    }

    [Test]
    public async Task SetPreconditionsHtmlThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(
            () => environment.Run(
                _ => AllureApi.SetPreconditionsHtml("<p>preconditions</p>")
            )
        ).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task SetPreconditionsHtmlAsyncThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(
            () => environment.RunAsync(
                _ => AllureApi.SetPreconditionsHtmlAsync(
                    "<p>preconditions</p>",
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
