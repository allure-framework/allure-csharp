using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Steps;

public class StepTests
{
    [Test]
    public async Task StepActionAddsStepResult()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        int calls = 0;

        var state = environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "step",
                _ => { calls++; }
            );
            return current.Runtime.ContextApi.CurrentState;
        });

        var step = await Assert.That(test.Steps).HasSingleItem();
        await Assert.That(step.Name).IsEqualTo("step");
        await Assert.That(step.Status).IsEqualTo(Status.Passed);
        await Assert.That(calls).IsEqualTo(1);
        await Assert.That(state.HasStep).IsFalse();
        await Assert.That(state.HasTest).IsTrue();
    }

    [Test]
    public async Task StepFunctionAddsStepResult()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        int calls = 0;

        var state = environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "step",
                _ => { calls++; return 1; }
            );
            return current.Runtime.ContextApi.CurrentState;
        });

        var step = await Assert.That(test.Steps).HasSingleItem();
        await Assert.That(step.Name).IsEqualTo("step");
        await Assert.That(step.Status).IsEqualTo(Status.Passed);
        await Assert.That(calls).IsEqualTo(1);
        await Assert.That(state.HasStep).IsFalse();
        await Assert.That(state.HasTest).IsTrue();
    }

    [Test]
    public async Task StepFunctionReturnsValue()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        var result = environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            return AllureInProcessApi.Step("step", _ => 42);
        });

        await Assert.That(result).IsEqualTo(42);
    }

    [Test]
    public async Task StepActionExceptionIsRethrownAndRecorded()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        Exception? exception = null;

        var state = environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            try
            {
                AllureInProcessApi.Step(
                    "step",
                    _ => throw new TestException("step failed")
                );
            }
            catch (Exception e)
            {
                exception = e;
            }
            return current.Runtime.ContextApi.CurrentState;
        });

        var step = await Assert.That(test.Steps).HasSingleItem();
        await Assert.That(step.Status).IsEqualTo(Status.Broken);
        await Assert.That(step.StatusDetails!.Message)
            .IsEqualTo("step failed");
        await Assert.That(state.HasStep).IsFalse();
        await Assert.That(state.HasTest).IsTrue();
    }

    [Test]
    public async Task StepFunctionExceptionIsRethrownAndRecorded()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        Exception? exception = null;

        var state = environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            try
            {
                AllureInProcessApi.Step(
                    "step",
                    _ =>
                    {
                        throw new TestException("step failed");
                        #pragma warning disable CS0162
                        return 1;
                        #pragma warning restore CS0162
                    }
                );
            }
            catch (Exception e)
            {
                exception = e;
            }
            return current.Runtime.ContextApi.CurrentState;
        });

        var step = await Assert.That(test.Steps).HasSingleItem();
        await Assert.That(step.Status).IsEqualTo(Status.Broken);
        await Assert.That(step.StatusDetails!.Message)
            .IsEqualTo("step failed");
        await Assert.That(state.HasStep).IsFalse();
        await Assert.That(state.HasTest).IsTrue();
    }

    [Test]
    public async Task StepAsyncActionAddsStepResult()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        int calls = 0;

        var state = await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                async _ => { calls++; }
            );
            return current.Runtime.ContextApi.CurrentState;
        });

        var step = await Assert.That(test.Steps).HasSingleItem();
        await Assert.That(step.Name).IsEqualTo("step");
        await Assert.That(step.Status).IsEqualTo(Status.Passed);
        await Assert.That(calls).IsEqualTo(1);
        await Assert.That(state.HasStep).IsFalse();
        await Assert.That(state.HasTest).IsTrue();
    }

    [Test]
    public async Task StepAsyncFunctionAddsStepResult()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        int calls = 0;

        var state = await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                async _ => { calls++; return 1; }
            );
            return current.Runtime.ContextApi.CurrentState;
        });

        var step = await Assert.That(test.Steps).HasSingleItem();
        await Assert.That(step.Name).IsEqualTo("step");
        await Assert.That(step.Status).IsEqualTo(Status.Passed);
        await Assert.That(calls).IsEqualTo(1);
        await Assert.That(state.HasStep).IsFalse();
        await Assert.That(state.HasTest).IsTrue();
    }

    [Test]
    public async Task StepAsyncFunctionReturnsValue()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        var result = await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            return await AllureInProcessApi.StepAsync("step", _ => Task.FromResult(42));
        });

        await Assert.That(result).IsEqualTo(42);
    }

    [Test]
    public async Task StepAsyncActionExceptionIsRethrownAndRecorded()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        Exception? exception = null;

        var state = await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            try
            {
                await AllureInProcessApi.StepAsync(
                    "step",
                    _ => throw new TestException("step failed")
                );
            }
            catch (Exception e)
            {
                exception = e;
            }
            return current.Runtime.ContextApi.CurrentState;
        });

        var step = await Assert.That(test.Steps).HasSingleItem();
        await Assert.That(step.Status).IsEqualTo(Status.Broken);
        await Assert.That(step.StatusDetails!.Message)
            .IsEqualTo("step failed");
        await Assert.That(state.HasStep).IsFalse();
        await Assert.That(state.HasTest).IsTrue();
    }

    [Test]
    public async Task StepAsyncFunctionExceptionIsRethrownAndRecorded()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        Exception? exception = null;

        var state = await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            try
            {
                _ = await AllureInProcessApi.StepAsync(
                    "step",
                    _ =>
                    {
                        throw new TestException("step failed");
                        #pragma warning disable CS0162
                        return Task.FromResult(1);
                        #pragma warning restore CS0162
                    }
                );
            }
            catch (Exception e)
            {
                exception = e;
            }
            return current.Runtime.ContextApi.CurrentState;
        });

        var step = await Assert.That(test.Steps).HasSingleItem();
        await Assert.That(step.Status).IsEqualTo(Status.Broken);
        await Assert.That(step.StatusDetails!.Message)
            .IsEqualTo("step failed");
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasTest).IsTrue();
    }

    [Test]
    public async Task SetUpAsyncActionReceivesToken()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        using var cancellation = new CancellationTokenSource();
        var observed = CancellationToken.None;

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                (_, token) =>
                {
                    observed = token;
                    return Task.CompletedTask;
                },
                cancellation.Token
            );
        });

        await Assert.That(observed).IsEqualTo(cancellation.Token);
    }

    [Test]
    public async Task StepAsyncFunctionReceivesToken()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();
        using var cancellation = new CancellationTokenSource();
        var observed = CancellationToken.None;

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                (_, token) =>
                {
                    observed = token;
                    return Task.FromResult(1);
                },
                cancellation.Token
            );
        });

        await Assert.That(observed).IsEqualTo(cancellation.Token);
    }

    [Test]
    public async Task StepAddsNestedStepToCurrentStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "outer",
                _ => AllureInProcessApi.Step("inner", _ => { })
            );
        });

        var outer = await Assert.That(test.Steps).HasSingleItem();
        var inner = await Assert.That(outer.Steps).HasSingleItem();
        await Assert.That(inner.Name).IsEqualTo("inner");
    }

    [Test]
    public async Task StepAsyncAddsNestedStepToCurrentStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "outer",
                async (_, token) => await AllureInProcessApi.StepAsync(
                    "inner",
                    (_, _) => Task.CompletedTask,
                    token
                ),
                CancellationToken.None
            );
        });

        var outer = await Assert.That(test.Steps).HasSingleItem();
        var inner = await Assert.That(outer.Steps).HasSingleItem();
        await Assert.That(inner.Name).IsEqualTo("inner");
    }

    [Test]
    public async Task StepAddsStepToCurrentFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp(
                "fixture",
                _ => AllureInProcessApi.Step("step", _ => { })
            );
        });

        await Assert.That(scope.Befores.Single().Steps).HasSingleItem();
    }

    [Test]
    public async Task StepAsyncAddsStepToCurrentFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "fixture",
                async (_, token) => await AllureInProcessApi.StepAsync(
                    "step",
                    (_, _) => Task.CompletedTask,
                    token
                ),
                CancellationToken.None
            );
        });

        await Assert.That(scope.Befores.Single().Steps).HasSingleItem();
    }

    [Test]
    public async Task StepPrioritizesCurrentFixtureOverTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.SetUp(
                "fixture",
                _ => AllureInProcessApi.Step("step", _ => { })
            );
        });

        await Assert.That(test.Steps).IsEmpty();
        await Assert.That(scope.Befores.Single().Steps).HasSingleItem();
    }

    [Test]
    public async Task StepAsyncPrioritizesCurrentFixtureOverTest()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        var test = NewTest();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.SetUpAsync(
                "fixture",
                async (_, token) => await AllureInProcessApi.StepAsync(
                    "step",
                    (_, _) => Task.CompletedTask,
                    token
                ),
                CancellationToken.None
            );
        });

        await Assert.That(test.Steps).IsEmpty();
        await Assert.That(scope.Befores.Single().Steps).HasSingleItem();
    }

    static AllureTestResult NewTest() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "test",
    };

    static TestResultScope NewScope() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "scope",
    };

    sealed class TestException(string message) : Exception(message);
}
