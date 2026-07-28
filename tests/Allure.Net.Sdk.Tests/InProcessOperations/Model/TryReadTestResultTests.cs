using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Model;

public class TryReadTestResultTests
{
    [Test]
    public async Task TryReadTestResultReturnsProjectedValue()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        var success = false;
        string? value = null;

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            success = AllureInProcessApi.TryReadTestResult(
                result => result.Name,
                out value
            );
        });

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo("test");
    }

    [Test]
    public async Task TryReadTestResultReturnsFalseIfNoTestRunning()
    {
        var environment = AllureApiTestEnvironment.Create();
        var readCalled = false;
        var success = true;
        string? value = "not-default";

        environment.Run(_ =>
        {
            success = AllureInProcessApi.TryReadTestResult(
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
