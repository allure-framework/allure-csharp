using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Fixtures;

public class TearDownTests
{
    [Test]
    public async Task TearDownActionAddsTearDownFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        int calls = 0;

        var state = environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            AllureInProcessApi.TearDown(
                "setup",
                _ => { calls++; }
            );
            return current.Runtime.ContextApi.CurrentState;
        });

        var fixture = await Assert.That(scope.Afters).HasSingleItem();
        await Assert.That(fixture.Name).IsEqualTo("setup");
        await Assert.That(fixture.Status).IsEqualTo(Status.Passed);
        await Assert.That(calls).IsEqualTo(1);
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task TearDownFunctionAddsTearDownFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        int calls = 0;

        var state = environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            AllureInProcessApi.TearDown(
                "setup",
                _ => calls++
            );
            return current.Runtime.ContextApi.CurrentState;
        });

        var fixture = await Assert.That(scope.Afters).HasSingleItem();
        await Assert.That(fixture.Name).IsEqualTo("setup");
        await Assert.That(fixture.Status).IsEqualTo(Status.Passed);
        await Assert.That(calls).IsEqualTo(1);
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task TearDownFunctionReturnsValue()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        var result = environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            return AllureInProcessApi.TearDown("setup", _ => 42);
        });

        await Assert.That(result).IsEqualTo(42);
    }

    [Test]
    public async Task TearDownActionExceptionIsRethrownAndRecorded()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        Exception? exception = null;

        var state = environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            try
            {
                AllureInProcessApi.TearDown(
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

        var fixture = await Assert.That(scope.Afters).HasSingleItem();
        await Assert.That(fixture.Status).IsEqualTo(Status.Broken);
        await Assert.That(fixture.StatusDetails!.Message)
            .IsEqualTo("setup failed");
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task TearDownFunctionExceptionIsRethrownAndRecorded()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        Exception? exception = null;

        var state = environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            try
            {
                _ = AllureInProcessApi.TearDown(
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

        var fixture = await Assert.That(scope.Afters).HasSingleItem();
        await Assert.That(fixture.Status).IsEqualTo(Status.Broken);
        await Assert.That(fixture.StatusDetails!.Message)
            .IsEqualTo("setup failed");
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task TearDownAsyncActionAddsTearDownFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        int calls = 0;

        var state = await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            await AllureInProcessApi.TearDownAsync(
                "setup",
                _ =>
                {
                    calls++;
                    return Task.CompletedTask;
                }
            );
            return current.Runtime.ContextApi.CurrentState;
        });

        var fixture = await Assert.That(scope.Afters).HasSingleItem();
        await Assert.That(fixture.Name).IsEqualTo("setup");
        await Assert.That(fixture.Status).IsEqualTo(Status.Passed);
        await Assert.That(calls).IsEqualTo(1);
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task TearDownAsyncFunctionAddsTearDownFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        int calls = 0;

        var state = await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            _ = await AllureInProcessApi.TearDownAsync(
                "setup",
                _ => Task.FromResult(calls++)
            );
            return current.Runtime.ContextApi.CurrentState;
        });

        var fixture = await Assert.That(scope.Afters).HasSingleItem();
        await Assert.That(fixture.Name).IsEqualTo("setup");
        await Assert.That(fixture.Status).IsEqualTo(Status.Passed);
        await Assert.That(calls).IsEqualTo(1);
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task TearDownAsyncFunctionReturnsValue()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        var result = await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            return await AllureInProcessApi.TearDownAsync("setup", _ => Task.FromResult(42));
        });

        await Assert.That(result).IsEqualTo(42);
    }

    [Test]
    public async Task TearDownAsyncActionExceptionIsRethrownAndRecorded()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        Exception? exception = null;

        var state = await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            try
            {
                await AllureInProcessApi.TearDownAsync(
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

        var fixture = await Assert.That(scope.Afters).HasSingleItem();
        await Assert.That(fixture.Status).IsEqualTo(Status.Broken);
        await Assert.That(fixture.StatusDetails!.Message)
            .IsEqualTo("setup failed");
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task TearDownAsyncFunctionExceptionIsRethrownAndRecorded()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        Exception? exception = null;

        var state = await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            try
            {
                _ = await AllureInProcessApi.TearDownAsync(
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

        var fixture = await Assert.That(scope.Afters).HasSingleItem();
        await Assert.That(fixture.Status).IsEqualTo(Status.Broken);
        await Assert.That(fixture.StatusDetails!.Message)
            .IsEqualTo("setup failed");
        await Assert.That(state.HasFixture).IsFalse();
        await Assert.That(state.HasScope).IsTrue();
    }

    [Test]
    public async Task TearDownAsyncActionReceivesToken()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        using var cancellation = new CancellationTokenSource();
        var observed = CancellationToken.None;

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            await AllureInProcessApi.TearDownAsync(
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
    public async Task TearDownAsyncFunctionReceivesToken()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();
        using var cancellation = new CancellationTokenSource();
        var observed = CancellationToken.None;

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTestScope(scope);
            await AllureInProcessApi.TearDownAsync(
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
