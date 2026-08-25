using Allure.Abstractions;
using Allure.Model;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Concurrency;

public class FacadeIsolationTests
{
    [Test]
    public async Task ParallelLogicalScopesAreIsolated()
    {
        var executions = Enumerable.Range(0, 24).Select(index => Task.Run(async () =>
        {
            var currentSync = RecordingInterface<IAllureSyncOperations<IAllureSyncStepContext, IAllureSyncFixtureContext>>.Create();
            var currentAsync = RecordingInterface<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>>.Create();
            var globalSync = RecordingInterface<IAllureSyncOperations<IAllureSyncStepContext, IAllureSyncFixtureContext>>.Create();
            var globalAsync = RecordingInterface<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>>.Create();
            using var scope = FacadeTestEnvironment.Use(
                new TestApiEndpoint(currentSync.Instance, currentAsync.Instance),
                new TestApiEndpoint(globalSync.Instance, globalAsync.Instance)
            );

            await Task.Yield();
            AllureApi.SetName($"sync-{index}");
            await AllureApi.SetNameAsync($"async-{index}");
            AllureApi.AddGlobalError($"global-sync-{index}");
            await AllureApi.AddGlobalErrorAsync($"global-async-{index}");
            await Task.Yield();

            return new
            {
                Index = index,
                CurrentSync = currentSync.Calls,
                CurrentAsync = currentAsync.Calls,
                GlobalSync = globalSync.Calls,
                GlobalAsync = globalAsync.Calls,
                scope.CurrentResolutionCount,
                scope.GlobalResolutionCount,
            };
        })).ToArray();

        var results = await Task.WhenAll(executions);

        foreach (var result in results)
        {
            await Assert.That(result.CurrentSync.Count).IsEqualTo(1);
            await Assert.That(result.CurrentSync[0].Arguments[0]).IsEqualTo($"sync-{result.Index}");
            await Assert.That(result.CurrentSync[0].Method.Name).IsEqualTo("SetName");

            await Assert.That(result.CurrentAsync).Count().IsEqualTo(1);
            await Assert.That(result.CurrentAsync[0].Arguments[0]).IsEqualTo($"async-{result.Index}");
            await Assert.That(result.CurrentAsync[0].Method.Name).IsEqualTo("SetNameAsync");

            await Assert.That(result.GlobalSync.Count).IsEqualTo(1);
            await Assert.That(((GlobalError)result.GlobalSync[0].Arguments[0]!).Message)
                .IsEqualTo($"global-sync-{result.Index}");
            await Assert.That(result.GlobalSync[0].Method.Name).IsEqualTo("AddGlobalError");

            await Assert.That(result.GlobalAsync.Count).IsEqualTo(1);
            await Assert.That(((GlobalError)result.GlobalAsync[0].Arguments[0]!).Message)
                .IsEqualTo($"global-async-{result.Index}");
            await Assert.That(result.GlobalAsync[0].Method.Name).IsEqualTo("AddGlobalErrorAsync");

            await Assert.That(result.CurrentResolutionCount).IsEqualTo(2);
            await Assert.That(result.GlobalResolutionCount).IsEqualTo(2);
        }
    }

    [Test]
    public async Task NestedScopeDoesNotOverwriteParentAsyncContext()
    {
        var parent = RecordingInterface<IAllureSyncOperations<IAllureSyncStepContext, IAllureSyncFixtureContext>>.Create();
        using var parentScope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: parent.Instance));

        AllureApi.SetName("parent-before");
        var childCall = await Task.Run(() =>
        {
            var child = RecordingInterface<IAllureSyncOperations<IAllureSyncStepContext, IAllureSyncFixtureContext>>.Create();
            using var childScope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: child.Instance));
            AllureApi.SetName("child");
            return child.SingleCall;
        });
        AllureApi.SetName("parent-after");

        await Assert.That(childCall.Arguments[0]).IsEqualTo("child");
        await Assert.That(parent.Calls.Count).IsEqualTo(2);
        await Assert.That(parent.Calls[0].Arguments[0]).IsEqualTo("parent-before");
        await Assert.That(parent.Calls[1].Arguments[0]).IsEqualTo("parent-after");
    }
}
