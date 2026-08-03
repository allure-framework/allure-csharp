using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Fixtures;

public class SetUpTests
{
    [Test]
    public async Task SetUpActionAddsSetUpFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        int calls = 0;

        var state = environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            AllureInProcessApi.SetUp(
                "setup",
                _ => { calls++; }
            );
            return current.Runtime.ContextApi.CurrentState;
        });

        var fixture = await Assert.That(scope.Befores).HasSingleItem();
        await Assert.That(fixture.Name).IsEqualTo("setup");
        await Assert.That(fixture.Status).IsEqualTo(Status.Passed);
        await Assert.That(calls).IsEqualTo(1);
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task SetUpFunctionAddsSetUpFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        int calls = 0;

        var state = environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            AllureInProcessApi.SetUp(
                "setup",
                _ => calls++
            );
            return current.Runtime.ContextApi.CurrentState;
        });

        var fixture = await Assert.That(scope.Befores).HasSingleItem();
        await Assert.That(fixture.Name).IsEqualTo("setup");
        await Assert.That(fixture.Status).IsEqualTo(Status.Passed);
        await Assert.That(calls).IsEqualTo(1);
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task SetUpFunctionReturnsValue()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        var result = environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            return AllureInProcessApi.SetUp("setup", _ => 42);
        });

        await Assert.That(result).IsEqualTo(42);
    }

    [Test]
    public async Task SetUpActionExceptionIsRethrownAndRecorded()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        Exception? exception = null;

        var state = environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            try
            {
                AllureInProcessApi.SetUp(
                    "setup",
                    _ => throw new TestException("setup failed")
                );
            }
            catch (Exception e)
            {
                exception = e;
            }
            return current.Runtime.ContextApi.CurrentState;
        });

        var fixture = await Assert.That(scope.Befores).HasSingleItem();
        await Assert.That(fixture.Status).IsEqualTo(Status.Broken);
        await Assert.That(fixture.StatusDetails!.Message)
            .IsEqualTo("setup failed");
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task SetUpFunctionExceptionIsRethrownAndRecorded()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        Exception? exception = null;

        var state = environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            try
            {
                _ = AllureInProcessApi.SetUp(
                    "setup",
                    _ =>
                    {
                        throw new TestException("setup failed");
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

        var fixture = await Assert.That(scope.Befores).HasSingleItem();
        await Assert.That(fixture.Status).IsEqualTo(Status.Broken);
        await Assert.That(fixture.StatusDetails!.Message)
            .IsEqualTo("setup failed");
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task SetUpAsyncActionAddsSetUpFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        int calls = 0;

        var state = await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "setup",
                _ =>
                {
                    calls++;
                    return Task.CompletedTask;
                }
            );
            return current.Runtime.ContextApi.CurrentState;
        });

        var fixture = await Assert.That(scope.Befores).HasSingleItem();
        await Assert.That(fixture.Name).IsEqualTo("setup");
        await Assert.That(fixture.Status).IsEqualTo(Status.Passed);
        await Assert.That(calls).IsEqualTo(1);
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task SetUpAsyncFunctionAddsSetUpFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        int calls = 0;

        var state = await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            _ = await AllureInProcessApi.SetUpAsync(
                "setup",
                _ => Task.FromResult(calls++)
            );
            return current.Runtime.ContextApi.CurrentState;
        });

        var fixture = await Assert.That(scope.Befores).HasSingleItem();
        await Assert.That(fixture.Name).IsEqualTo("setup");
        await Assert.That(fixture.Status).IsEqualTo(Status.Passed);
        await Assert.That(calls).IsEqualTo(1);
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task SetUpAsyncFunctionReturnsValue()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        var result = await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            return await AllureInProcessApi.SetUpAsync("setup", _ => Task.FromResult(42));
        });

        await Assert.That(result).IsEqualTo(42);
    }

    [Test]
    public async Task SetUpAsyncActionExceptionIsRethrownAndRecorded()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        Exception? exception = null;

        var state = await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            try
            {
                await AllureInProcessApi.SetUpAsync(
                    "setup",
                    _ => throw new TestException("setup failed")
                );
            }
            catch (Exception e)
            {
                exception = e;
            }
            return current.Runtime.ContextApi.CurrentState;
        });

        var fixture = await Assert.That(scope.Befores).HasSingleItem();
        await Assert.That(fixture.Status).IsEqualTo(Status.Broken);
        await Assert.That(fixture.StatusDetails!.Message)
            .IsEqualTo("setup failed");
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task SetUpAsyncFunctionExceptionIsRethrownAndRecorded()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        Exception? exception = null;

        var state = await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            try
            {
                _ = await AllureInProcessApi.SetUpAsync(
                    "setup",
                    _ =>
                    {
                        throw new TestException("setup failed");
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

        var fixture = await Assert.That(scope.Befores).HasSingleItem();
        await Assert.That(fixture.Status).IsEqualTo(Status.Broken);
        await Assert.That(fixture.StatusDetails!.Message)
            .IsEqualTo("setup failed");
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task SetUpAsyncActionReceivesToken()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        using var cancellation = new CancellationTokenSource();
        var observed = CancellationToken.None;

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "setup",
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
    public async Task SetUpAsyncFunctionReceivesToken()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        using var cancellation = new CancellationTokenSource();
        var observed = CancellationToken.None;

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "setup",
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

    static TestResultScope NewScope() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "scope",
    };

    sealed class TestException(string message) : Exception(message);
}
