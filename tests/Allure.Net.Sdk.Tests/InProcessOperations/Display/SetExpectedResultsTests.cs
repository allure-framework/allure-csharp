using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Display;

public class SetExpectedResultsTests
{
    [Test]
    public async Task SetExpectedResultsUpdatesCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.SetExpectedResults("expected results");
        });

        await Assert.That(test.ExpectedResults).IsEqualTo("expected results");
    }

    [Test]
    public async Task SetExpectedResultsAsyncUpdatesCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.SetExpectedResultsAsync(
                "expected results",
                CancellationToken.None
            );
        });

        await Assert.That(test.ExpectedResults).IsEqualTo("expected results");
    }

    [Test]
    public async Task SetExpectedResultsThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(
            () => environment.Run(
                _ => AllureApi.SetExpectedResults("expected results")
            )
        ).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task SetExpectedResultsAsyncThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(
            () => environment.RunAsync(
                _ => AllureApi.SetExpectedResultsAsync(
                    "expected results",
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
