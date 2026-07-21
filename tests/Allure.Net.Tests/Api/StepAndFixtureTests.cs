using Allure.Abstractions;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Api;

public class StepAndFixtureTests
{
    [Test]
    public async Task StepForwardsBodyAndReturnsOperationResult()
    {
        var operations = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        Func<int> body = () => 17;
        operations.Handler = (_, arguments) => ((Func<int>)arguments[2]!)() + 1;
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: operations.Instance));

        var result = AllureApi.Step("calculation", body);

        await Assert.That(result).IsEqualTo(18);
        await Assert.That(operations.SingleCall.Method.Name).IsEqualTo("Step");
        await Assert.That(operations.SingleCall.Arguments[0]).IsEqualTo("calculation");
        await Assert.That(operations.SingleCall.Arguments[2]).IsSameReferenceAs(body);
    }

    [Test]
    public async Task AsyncStepForwardsBodyAndReturnsOperationResult()
    {
        var operations = RecordingInterface<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>>.Create();
        Func<Task<int>> body = () => Task.FromResult(17);
        operations.Handler = (_, arguments) => AddOne((Func<Task<int>>)arguments[2]!);
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(@async: operations.Instance));
        using var cancellation = new CancellationTokenSource();

        var result = await AllureApi.StepAsync("calculation", body, cancellation.Token);

        await Assert.That(result).IsEqualTo(18);
        await Assert.That(operations.SingleCall.Method.Name).IsEqualTo("StepAsync");
        await Assert.That(operations.SingleCall.Arguments[2]).IsSameReferenceAs(body);
        await Assert.That(operations.SingleCall.Arguments[3]).IsEqualTo(cancellation.Token);
    }

    static async Task<int> AddOne(Func<Task<int>> body) => await body() + 1;

    [Test]
    public async Task BodiesExecuteDirectlyWhenCurrentEndpointIsMissing()
    {
        using var scope = FacadeTestEnvironment.Use();
        var stepCalls = 0;
        var fixtureCalls = 0;

        var stepResult = AllureApi.Step("step", () => { stepCalls++; return 3; });
        var fixtureResult = await AllureApi.SetUpAsync("fixture", async () =>
        {
            await Task.Yield();
            fixtureCalls++;
            return 4;
        });

        await Assert.That(stepResult).IsEqualTo(3);
        await Assert.That(fixtureResult).IsEqualTo(4);
        await Assert.That(stepCalls).IsEqualTo(1);
        await Assert.That(fixtureCalls).IsEqualTo(1);
        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(2);
    }

    [Test]
    public async Task EmptyStepForwardsDefaultStatusAndNoDetails()
    {
        var operations = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: operations.Instance));

        AllureApi.Step("empty");

        await Assert.That(operations.SingleCall.Arguments[2]).IsEqualTo(Allure.Model.Status.Passed);
        await Assert.That(operations.SingleCall.Arguments[3]).IsNull();
    }
}
