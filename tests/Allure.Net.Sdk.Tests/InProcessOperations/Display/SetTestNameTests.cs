using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Display;

public class SetTestNameTests
{
    [Test]
    public async Task SetTestNameRenamesCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.SetTestName("renamed");
        });

        await Assert.That(test.Name).IsEqualTo("renamed");
    }

    [Test]
    public async Task SetTestNameAsyncRenamesCurrentTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.SetTestNameAsync(
                "renamed",
                CancellationToken.None
            );
        });

        await Assert.That(test.Name).IsEqualTo("renamed");
    }

    [Test]
    public async Task SetTestNameThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(
            () => environment.Run(_ => AllureApi.SetTestName("never"))
        ).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task SetTestNameAsyncThrowsIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();

        await Assert.That(
            () => environment.RunAsync(
                _ => AllureApi.SetTestNameAsync(
                    "never",
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
