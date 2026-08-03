using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Model;

public class TryReadFixtureResultTests
{
    [Test]
    public async Task TryReadFixtureResultReturnsProjectedValue()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        var success = false;
        string? value = null;

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            AllureInProcessApi.SetUp("fixture", _ =>
            {
                success = AllureInProcessApi.TryReadFixtureResult(
                    result => result.Name,
                    out value
                );
            });
        });

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo("fixture");
    }

    [Test]
    public async Task TryReadFixtureResultReturnsFalseIfNoFixtureRunning()
    {
        var environment = AllureApiTestEnvironment.Create();
        var readCalled = false;
        var success = true;
        string? value = "not-default";

        environment.Run(_ =>
        {
            success = AllureInProcessApi.TryReadFixtureResult(
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

    static TestResultScope NewScope() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "scope",
    };
}
