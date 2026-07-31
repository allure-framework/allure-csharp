using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Model;

public class TryReadStepResultTests
{
    [Test]
    public async Task TryReadStepResultReturnsProjectedValue()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        var success = false;
        string? value = null;

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step("step", _ =>
            {
                success = AllureInProcessApi.TryReadStepResult(
                    result => result.Name,
                    out value
                );
            });
        });

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo("step");
    }

    [Test]
    public async Task TryReadStepResultReturnsFalseIfNoStepRunning()
    {
        var environment = AllureApiTestEnvironment.Create();
        var readCalled = false;
        var success = true;
        string? value = "not-default";

        environment.Run(_ =>
        {
            success = AllureInProcessApi.TryReadStepResult(
                result =>
                {
                    readCalled = true;
                    return result.Name;
                },
                out value
            );
        });

        await Assert.That(success).IsFalse();
        await Assert.That(value).IsNull();
        await Assert.That(readCalled).IsFalse();
    }

    static AllureTestResult NewTest() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "test",
    };
}
