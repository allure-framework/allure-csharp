using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Display;

public class SetPreconditionsTests
{
    [Test]
    public async Task SetPreconditionsUpdatesCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.SetPreconditions("preconditions");
        });

        await Assert.That(test.Preconditions).IsEqualTo("preconditions");
    }

    [Test]
    public async Task SetPreconditionsAsyncUpdatesCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.SetPreconditionsAsync(
                "preconditions",
                CancellationToken.None
            );
        });

        await Assert.That(test.Preconditions).IsEqualTo("preconditions");
    }

    [Test]
    public async Task SetPreconditionsThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(
            () => environment.Run(
                _ => AllureApi.SetPreconditions("preconditions")
            )
        ).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task SetPreconditionsAsyncThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(
            () => environment.RunAsync(
                _ => AllureApi.SetPreconditionsAsync(
                    "preconditions",
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
