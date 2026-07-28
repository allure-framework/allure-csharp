using Allure.Abstractions;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Steps;

public class StepContextTests
{
    [Test]
    public async Task SetNameRenamesBoundStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "step",
                context => context.SetName("renamed")
            );
        });

        await Assert.That(test.Steps.Single().Name).IsEqualTo("renamed");
    }

    [Test]
    public async Task ShouldAddParameterToBoundStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "step",
                context => context.AddParameter(new (){ Name = "Foo", Value = "Bar" })
            );
        });

        var step = await Assert.That(test.Steps).HasSingleItem();
        var parameter = await Assert.That(step.Parameters).HasSingleItem();
        await Assert.That(parameter.Name).IsEqualTo("Foo");
        await Assert.That(parameter.Value).IsEqualTo("Bar");
    }

    [Test]
    public async Task SetNameAsyncRenamesBoundStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                context => context.SetNameAsync("renamed")
            );
        });

        await Assert.That(test.Steps.Single().Name).IsEqualTo("renamed");
    }

    [Test]
    public async Task ShouldAddParameterToBoundAsyncStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                context => context.AddParameterAsync(new (){ Name = "Foo", Value = "Bar" })
            );
        });

        var step = await Assert.That(test.Steps).HasSingleItem();
        var parameter = await Assert.That(step.Parameters).HasSingleItem();
        await Assert.That(parameter.Name).IsEqualTo("Foo");
        await Assert.That(parameter.Value).IsEqualTo("Bar");
    }

    [Test]
    public async Task CanUpdateBoundStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "step",
                context => context.UpdateStepResult(static (step) =>
                {
                    step.Name = "updated";
                })
            );
        });

        await Assert.That(test.Steps.Single().Name).IsEqualTo("updated");
    }

    [Test]
    public async Task CanReadBoundStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "step",
                context => context.ReadStepResult(
                    static (step) => step.Name
                )
            );
        });

        await Assert.That(test.Steps.Single().Name).IsEqualTo("step");
    }

    [Test]
    public async Task CanUpdateBoundAsyncStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                async context => context.UpdateStepResult(static (step) =>
                {
                    step.Name = "updated";
                })
            );
        });

        await Assert.That(test.Steps.Single().Name).IsEqualTo("updated");
    }

    [Test]
    public async Task CanReadBoundAsyncStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                async context => context.ReadStepResult(
                    static (fixture) => fixture.Name
                )
            );
        });

        await Assert.That(test.Steps.Single().Name).IsEqualTo("step");
    }

    [Test]
    public async Task CompletedActionStepCannotBeRenamed()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "step",
                ctx => { context = ctx; }
            );
            AllureInProcessApi.Step(
                "step",
                ctx => context!.SetName("never")
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    [Test]
    public async Task CompletedFunctionStepCannotBeRenamed()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "step",
                ctx => { context = ctx; return 1; }
            );
            AllureInProcessApi.Step(
                "step",
                ctx => context!.SetName("never")
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    [Test]
    public async Task CompletedAsyncActionStepCannotBeRenamed()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                async ctx => { context = ctx; }
            );
            await AllureInProcessApi.StepAsync(
                "step",
                async ctx => await context!.SetNameAsync("never")
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    [Test]
    public async Task CompletedAsyncFunctionStepCannotBeRenamed()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                async ctx => { context = ctx; return 1; }
            );
            await AllureInProcessApi.StepAsync(
                "step",
                async ctx => await context!.SetNameAsync("never")
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotAddParameterToCompletedActionStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "step",
                ctx => { context = ctx; }
            );
            AllureInProcessApi.Step(
                "step",
                ctx => context!.AddParameter(new(){ Name = "foo", Value = "bar" })
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotAddParameterToCompletedFunctionStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "step",
                ctx => { context = ctx; return 1; }
            );
            AllureInProcessApi.Step(
                "step",
                ctx => context!.AddParameter(new(){ Name = "foo", Value = "bar" })
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotAddParameterToCompletedAsyncActionStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                async ctx => { context = ctx; }
            );
            await AllureInProcessApi.StepAsync(
                "step",
                async ctx => await context!.AddParameterAsync(new(){ Name = "foo", Value = "bar" })
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotAddParameterToCompletedAsyncFunctionStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                async ctx => { context = ctx; return 1; }
            );
            await AllureInProcessApi.StepAsync(
                "step",
                async ctx => await context!.AddParameterAsync(new(){ Name = "foo", Value = "bar" })
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotUpdateCompletedActionStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "step",
                ctx => { context = ctx; }
            );
            AllureInProcessApi.Step(
                "step",
                ctx => context!.UpdateStepResult(_ => {})
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotUpdateCompletedFunctionStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "step",
                ctx => { context = ctx; return 1; }
            );
            AllureInProcessApi.Step(
                "step",
                ctx => context!.UpdateStepResult(_ => {})
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotUpdateCompletedAsyncActionStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                async ctx => { context = ctx; }
            );
            AllureInProcessApi.Step(
                "step",
                ctx => context!.UpdateStepResult(_ => {})
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotUpdateCompletedAsyncFunctionStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                async ctx => { context = ctx; return 1; }
            );
            AllureInProcessApi.Step(
                "step",
                ctx => context!.UpdateStepResult(_ => {})
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotReadFromCompletedActionStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "step",
                ctx => { context = ctx; }
            );
            AllureInProcessApi.Step(
                "step",
                ctx => context!.ReadStepResult(_ => 1)
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotReadCompletedFunctionStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.Run(current =>
        {
            IAllureInProcessSyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            AllureInProcessApi.Step(
                "step",
                ctx => { context = ctx; return 1; }
            );
            AllureInProcessApi.Step(
                "step",
                ctx => context!.ReadStepResult(_ => 1)
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotReadCompletedAsyncActionStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                async ctx => { context = ctx; }
            );
            AllureInProcessApi.Step(
                "step",
                ctx => context!.ReadStepResult(_ => 1)
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    [Test]
    public async Task CannotReadCompletedAsyncFunctionStep()
    {
        var environment = AllureApiTestEnvironment.Create();
        var test = NewTest();

        await Assert.That(() => environment.RunAsync(async current =>
        {
            IAllureInProcessAsyncStepContext? context = null;
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureInProcessApi.StepAsync(
                "step",
                async ctx => { context = ctx; return 1; }
            );
            AllureInProcessApi.Step(
                "step",
                ctx => context!.ReadStepResult(_ => 1)
            );
        })).Throws<InvalidOperationException>()
            .WithMessage(
                "The step associated with this context has already finished."
            );
    }

    static AllureTestResult NewTest() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "test",
    };
}
