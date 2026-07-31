using Allure.Model;
using Allure.Sdk.Runtime;
using Allure.Net.Sdk.Tests.Infrastructure;

namespace Allure.Net.Sdk.Tests.Runtime;

public class AsyncLocalExecutionContextTests
{
    [Test]
    public async Task ShouldRestoreStateAfterSynchronousSuccess()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        var original = runtime.ContextApi.CurrentState;
        var state = CaptureTestState(runtime, "temporary");

        var name = runtime.ContextApi.GetWithState(
            state,
            current => current.ModelApi.ReadTestResult(test => test.Name)
        );

        await Assert.That(name).IsEqualTo("temporary");
        await Assert.That(runtime.ContextApi.CurrentState).IsEqualTo(original);
    }

    [Test]
    public async Task ShouldRestoreStateAfterSynchronousException()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        var original = runtime.ContextApi.CurrentState;
        var state = CaptureTestState(runtime, "temporary");

        await Assert.That(() => runtime.ContextApi.GetWithState<object>(
            state,
            _ => throw new TestException()
        )).Throws<TestException>();
        await Assert.That(runtime.ContextApi.CurrentState).IsEqualTo(original);
    }

    [Test]
    public async Task ShouldReturnStateProducedBySynchronousAction()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;

        var produced = runtime.ContextApi.RunWithState(
            new(),
            current => current.LifecycleApi.StartTest(
                new() { Uuid = "produced", Name = "produced" }
            )
        );

        await Assert.That(produced.HasTest).IsTrue();
        var name = runtime.ContextApi.GetWithState(
            produced,
            current => current.ModelApi.ReadTestResult(test => test.Name)
        );
        await Assert.That(name).IsEqualTo("produced");
        await Assert.That(runtime.ContextApi.CurrentState.HasTest).IsFalse();
    }

    [Test]
    public async Task ShouldRestoreStateAndReturnResultAfterAsyncSuccess()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        var original = runtime.ContextApi.CurrentState;
        var state = CaptureTestState(runtime, "temporary");

        var name = await runtime.ContextApi.GetWithStateAsync(
            state,
            async current =>
            {
                await Task.Yield();
                return current.ModelApi.ReadTestResult(test => test.Name);
            }
        );

        await Assert.That(name).IsEqualTo("temporary");
        await Assert.That(runtime.ContextApi.CurrentState).IsEqualTo(original);
    }

    [Test]
    public async Task ShouldRestoreStateAfterAsyncException()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        var original = runtime.ContextApi.CurrentState;
        var state = CaptureTestState(runtime, "temporary");

        await Assert.That(async () => await runtime.ContextApi.RunWithStateAsync(
            state,
            async _ =>
            {
                await Task.Yield();
                throw new TestException();
            }
        )).Throws<TestException>();
        await Assert.That(runtime.ContextApi.CurrentState).IsEqualTo(original);
    }

    [Test]
    public async Task ShouldRestoreNestedTemporaryStates()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        var original = runtime.ContextApi.CurrentState;
        var outer = CaptureTestState(runtime, "outer");
        var inner = CaptureTestState(runtime, "inner");

        var names = runtime.ContextApi.GetWithState(outer, current =>
        {
            var outerName = current.ModelApi.ReadTestResult(test => test.Name);
            var innerName = current.ContextApi.GetWithState(
                inner,
                nested => nested.ModelApi.ReadTestResult(test => test.Name)
            );
            var restoredOuterName =
                current.ModelApi.ReadTestResult(test => test.Name);
            return new[] { outerName, innerName, restoredOuterName };
        });

        await Assert.That(names).IsEquivalentTo(["outer", "inner", "outer"]);
        await Assert.That(runtime.ContextApi.CurrentState).IsEqualTo(original);
    }

    [Test]
    public async Task ShouldIsolateConcurrentAsyncFlows()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        var firstState = CaptureTestState(runtime, "first");
        var secondState = CaptureTestState(runtime, "second");
        var release = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        var entered = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        var enteredCount = 0;

        Task<string> ReadName(AllureExecutionState state) =>
            runtime.ContextApi.GetWithStateAsync(state, async current =>
            {
                if (Interlocked.Increment(ref enteredCount) == 2)
                {
                    entered.SetResult();
                }
                await release.Task;
                return current.ModelApi.ReadTestResult(test => test.Name);
            });

        var first = ReadName(firstState);
        var second = ReadName(secondState);
        await entered.Task;
        release.SetResult();

        await Assert.That(await first).IsEqualTo("first");
        await Assert.That(await second).IsEqualTo("second");
        await Assert.That(runtime.ContextApi.CurrentState.HasTest).IsFalse();
    }

    static AllureExecutionState CaptureTestState(
        IAllureRuntime runtime,
        string name
    )
    {
        runtime.LifecycleApi.StartTest(new()
        {
            Uuid = name,
            Name = name,
        });
        var state = runtime.ContextApi.CurrentState;
        runtime.LifecycleApi.StopTest();
        return state;
    }

    sealed class TestException : Exception;
}
