using Allure.Abstractions;
using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Fixtures;

public class FixtureContextTests
{
    [Test]
    public async Task SetNameRenamedBoundFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp(
                "setup",
                context => context.SetName("renamed")
            );
        });

        await Assert.That(scope.Befores.Single().Name).IsEqualTo("renamed");
    }

    [Test]
    public async Task ShouldAddParameterToBoundFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp(
                "setup",
                context => context.AddParameter(new (){ Name = "Foo", Value = "Bar" })
            );
        });

        var fixture = await Assert.That(scope.Befores).HasSingleItem();
        var parameter = await Assert.That(fixture.Parameters).HasSingleItem();
        await Assert.That(parameter.Name).IsEqualTo("Foo");
        await Assert.That(parameter.Value).IsEqualTo("Bar");
    }

    [Test]
    public async Task SetNameAsyncRenamedBoundFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "setup",
                context => context.SetNameAsync("renamed")
            );
        });

        await Assert.That(scope.Befores.Single().Name).IsEqualTo("renamed");
    }

    [Test]
    public async Task ShouldAddParameterToBoundAsyncFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "setup",
                context => context.AddParameterAsync(new (){ Name = "Foo", Value = "Bar" })
            );
        });

        var fixture = await Assert.That(scope.Befores).HasSingleItem();
        var parameter = await Assert.That(fixture.Parameters).HasSingleItem();
        await Assert.That(parameter.Name).IsEqualTo("Foo");
        await Assert.That(parameter.Value).IsEqualTo("Bar");
    }

    [Test]
    public async Task CanUpdateBoundFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp(
                "setup",
                context => context.UpdateFixtureResult(static (fixture) =>
                {
                    fixture.Name = "updated";
                })
            );
        });

        await Assert.That(scope.Befores.Single().Name).IsEqualTo("updated");
    }

    [Test]
    public async Task CanReadBoundFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp(
                "setup",
                context => context.ReadFixtureResult(
                    static (fixture) => fixture.Name
                )
            );
        });

        await Assert.That(scope.Befores.Single().Name).IsEqualTo("setup");
    }

    [Test]
    public async Task CanUpdateBoundAsyncFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "setup",
                async context => context.UpdateFixtureResult(static (fixture) =>
                {
                    fixture.Name = "updated";
                })
            );
        });

        await Assert.That(scope.Befores.Single().Name).IsEqualTo("updated");
    }

    [Test]
    public async Task CanReadBoundAsyncFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "setup",
                async context => context.ReadFixtureResult(
                    static (fixture) => fixture.Name
                )
            );
        });

        await Assert.That(scope.Befores.Single().Name).IsEqualTo("setup");
    }

    [Test]
    public async Task CompletedActionFixtureCannotBeRenamed()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp(
                "setup",
                ctx => { context = ctx; }
            );
            AllureInProcessApi.SetUp(
                "setup",
                ctx => context!.SetName("never")
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CompletedFunctionFixtureCannotBeRenamed()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp(
                "setup",
                ctx => { context = ctx; return 1; }
            );
            AllureInProcessApi.SetUp(
                "setup",
                ctx => context!.SetName("never")
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CompletedAsyncActionFixtureCannotBeRenamed()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "setup",
                async ctx => { context = ctx; }
            );
            await AllureInProcessApi.SetUpAsync(
                "setup",
                async ctx => await context!.SetNameAsync("never")
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CompletedAsyncFunctionFixtureCannotBeRenamed()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "setup",
                async ctx => { context = ctx; return 1; }
            );
            await AllureInProcessApi.SetUpAsync(
                "setup",
                async ctx => await context!.SetNameAsync("never")
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotAddParameterToCompletedActionFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp(
                "setup",
                ctx => { context = ctx; }
            );
            AllureInProcessApi.SetUp(
                "setup",
                ctx => context!.AddParameter(new(){ Name = "foo", Value = "bar" })
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotAddParameterToCompletedFunctionFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp(
                "setup",
                ctx => { context = ctx; return 1; }
            );
            AllureInProcessApi.SetUp(
                "setup",
                ctx => context!.AddParameter(new(){ Name = "foo", Value = "bar" })
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotAddParameterToCompletedAsyncActionFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "setup",
                async ctx => { context = ctx; }
            );
            await AllureInProcessApi.SetUpAsync(
                "setup",
                async ctx => await context!.AddParameterAsync(new(){ Name = "foo", Value = "bar" })
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotAddParameterToCompletedAsyncFunctionFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "setup",
                async ctx => { context = ctx; return 1; }
            );
            await AllureInProcessApi.SetUpAsync(
                "setup",
                async ctx => await context!.AddParameterAsync(new(){ Name = "foo", Value = "bar" })
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotUpdateCompletedActionFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp(
                "setup",
                ctx => { context = ctx; }
            );
            AllureInProcessApi.SetUp(
                "setup",
                ctx => context!.UpdateFixtureResult(_ => {})
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotUpdateCompletedFunctionFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp(
                "setup",
                ctx => { context = ctx; return 1; }
            );
            AllureInProcessApi.SetUp(
                "setup",
                ctx => context!.UpdateFixtureResult(_ => {})
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotUpdateCompletedAsyncActionFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "setup",
                async ctx => { context = ctx; }
            );
            AllureInProcessApi.SetUp(
                "setup",
                ctx => context!.UpdateFixtureResult(_ => {})
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotUpdateCompletedAsyncFunctionFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "setup",
                async ctx => { context = ctx; return 1; }
            );
            AllureInProcessApi.SetUp(
                "setup",
                ctx => context!.UpdateFixtureResult(_ => {})
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotReadFromCompletedActionFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp(
                "setup",
                ctx => { context = ctx; }
            );
            AllureInProcessApi.SetUp(
                "setup",
                ctx => context!.ReadFixtureResult(_ => 1)
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotReadCompletedFunctionFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.SetUp(
                "setup",
                ctx => { context = ctx; return 1; }
            );
            AllureInProcessApi.SetUp(
                "setup",
                ctx => context!.ReadFixtureResult(_ => 1)
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotReadCompletedAsyncActionFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "setup",
                async ctx => { context = ctx; }
            );
            AllureInProcessApi.SetUp(
                "setup",
                ctx => context!.ReadFixtureResult(_ => 1)
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotReadCompletedAsyncFunctionFixture()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.SetUpAsync(
                "setup",
                async ctx => { context = ctx; return 1; }
            );
            AllureInProcessApi.SetUp(
                "setup",
                ctx => context!.ReadFixtureResult(_ => 1)
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotChangeActionTearDownFixtureAfterCompletion()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.TearDown(
                "teardown",
                ctx => { context = ctx; }
            );
            AllureInProcessApi.SetUp(
                "teardown",
                ctx => context!.SetName("never")
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotChangeFunctionTearDownFixtureAfterCompletion()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            AllureInProcessApi.TearDown(
                "teardown",
                ctx => { context = ctx; return 1; }
            );
            AllureInProcessApi.SetUp(
                "teardown",
                ctx => context!.SetName("never")
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotChangeAsyncActionTearDownFixtureAfterCompletion()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.TearDownAsync(
                "teardown",
                async ctx => { context = ctx; }
            );
            await AllureInProcessApi.SetUp(
                "teardown",
                async ctx => await context!.SetNameAsync("never")
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotChangeAsyncFunctionTearDownFixtureAfterCompletion()
    {
        var environment = AllureApiTestEnvironment.Create();
        var scope = NewScope();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncFixtureContext? context = null;
            current.Runtime.LifecycleApi.StartScope(scope);
            await AllureInProcessApi.TearDownAsync(
                "teardown",
                async ctx => { context = ctx; return 1; }
            );
            await AllureInProcessApi.SetUp(
                "teardown",
                async ctx => await context!.SetNameAsync("never")
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The fixture associated with this context has already finished."
            );
    }

    static TestResultScope NewScope() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "scope",
    };
}
