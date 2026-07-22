using Allure.Abstractions;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Api;

public class InProcessApiTests
{
    [Test]
    public async Task UpdateForwardsDelegateToInProcessOperations()
    {
        var operations = RecordingInterface<IAllureInProcessOperations>.Create();
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: operations.Instance));
        Action<Allure.Model.TestResult> update = result => result.Name = "updated";

        AllureInProcessApi.UpdateTestResult(update);

        await Assert.That(operations.SingleCall.Method.Name).IsEqualTo("UpdateTestResult");
        await Assert.That(operations.SingleCall.Arguments[0]).IsSameReferenceAs(update);
        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(1);
    }

    [Test]
    public async Task ReadReturnsValueFromInProcessOperations()
    {
        var operations = RecordingInterface<IAllureInProcessOperations>.Create();
        operations.Handler = (_, arguments) =>
        {
            arguments[1] = "runtime value";
            return true;
        };
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: operations.Instance));

        var result = AllureInProcessApi.ReadTestResult(test => test.Name);

        await Assert.That(result).IsEqualTo("runtime value");
        await Assert.That(operations.SingleCall.Method.Name).IsEqualTo("TryReadTestResult");
    }

    [Test]
    public async Task ReadReturnsFallbackWithoutInvokingFactoryWhenValueExists()
    {
        var operations = RecordingInterface<IAllureInProcessOperations>.Create();
        operations.Handler = (_, arguments) =>
        {
            arguments[1] = 42;
            return true;
        };
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: operations.Instance));
        var fallbackCalled = false;

        var result = AllureInProcessApi.ReadFixtureResult(
            fixture => fixture.Name.Length,
            () => { fallbackCalled = true; return -1; }
        );

        await Assert.That(result).IsEqualTo(42);
        await Assert.That(fallbackCalled).IsFalse();
    }

    [Test]
    public async Task ReadInvokesFallbackFactoryWhenValueIsMissing()
    {
        var operations = RecordingInterface<IAllureInProcessOperations>.Create();
        operations.Handler = (_, arguments) =>
        {
            arguments[1] = default(int);
            return false;
        };
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: operations.Instance));
        var fallbackCalls = 0;

        var result = AllureInProcessApi.ReadStepResult(
            step => step.Name.Length,
            () => { fallbackCalls++; return 7; }
        );

        await Assert.That(result).IsEqualTo(7);
        await Assert.That(fallbackCalls).IsEqualTo(1);
    }

    [Test]
    public async Task RequiredReadThrowsWhenValueIsMissing()
    {
        var operations = RecordingInterface<IAllureInProcessOperations>.Create();
        operations.Handler = (_, arguments) =>
        {
            arguments[1] = null;
            return false;
        };
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: operations.Instance));

        await Assert.That(() => AllureInProcessApi.ReadFixtureResult(fixture => fixture.Name))
            .Throws<InvalidOperationException>()
            .WithMessage("Cannot read fixture result: no fixture is currently running.");
    }

    [Test]
    public async Task MissingEndpointMakesInProcessUpdateANoOp()
    {
        using var scope = FacadeTestEnvironment.Use();

        AllureInProcessApi.UpdateStepResult(_ => { });

        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(0);
    }

    [Test]
    public async Task RemoteEndpointReportsInProcessApiAsUnsupported()
    {
        using var scope = FacadeTestEnvironment.Use(current: new TestRuntime("remote endpoint"));

        await Assert.That(() => AllureInProcessApi.UpdateStepResult(_ => { }))
            .Throws<InvalidOperationException>()
            .WithMessage(
                "The current Allure runtime endpoint 'remote endpoint' "
                    + "does not support in-process model access."
            );
    }
}
