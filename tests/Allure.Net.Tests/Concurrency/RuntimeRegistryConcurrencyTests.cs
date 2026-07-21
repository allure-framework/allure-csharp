using System.Collections.Concurrent;
using Allure.Net.Tests.Infrastructure;
using Allure.Runtime;

namespace Allure.Net.Tests.Concurrency;

public class RuntimeRegistryConcurrencyTests
{
    [Test]
    public async Task SameRouteCanBeInstalledOnlyOnceConcurrently()
    {
        var registry = new AllureRuntimeRegistry();
        var route = new TestRuntimeRoute("route", new TestRuntime("runtime"));
        const int attemptCount = 16;
        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var registrations = new ConcurrentBag<IDisposable>();
        var failures = new ConcurrentBag<Exception>();

        var attempts = Enumerable.Range(0, attemptCount).Select(_ => Task.Run(async () =>
        {
            await start.Task;
            try
            {
                registrations.Add(registry.Install(route));
            }
            catch (Exception exception)
            {
                failures.Add(exception);
            }
        })).ToArray();
        start.SetResult();
        await Task.WhenAll(attempts);

        await Assert.That(registrations.Count).IsEqualTo(1);
        await Assert.That(failures.Count).IsEqualTo(attemptCount - 1);
        await Assert.That(failures.All(error => error is InvalidOperationException)).IsTrue();
        registrations.Single().Dispose();
    }

    [Test]
    public async Task ConcurrentInstallationAndDisposalLeaveRegistryEmpty()
    {
        var registry = new AllureRuntimeRegistry();
        var routes = Enumerable.Range(0, 64)
            .Select(index => new TestRuntimeRoute($"route-{index}", new TestRuntime($"runtime-{index}")))
            .ToArray();

        var registrations = await Task.WhenAll(routes.Select(route => Task.Run(() => registry.Install(route))));
        await Task.WhenAll(registrations.Select(registration => Task.Run(registration.Dispose)));

        await Assert.That(registry.ResolveCurrentScope()).IsNull();
        await Assert.That(registry.ResolveGlobalScope()).IsNull();
    }

    [Test]
    public async Task ResolutionUsesStableSnapshotDuringConcurrentDisposal()
    {
        var registry = new AllureRuntimeRegistry();
        var runtime = new TestRuntime("runtime");
        using var predicateEntered = new ManualResetEventSlim();
        using var releasePredicate = new ManualResetEventSlim();
        var registration = registry.Install(new TestRuntimeRoute(
            "route",
            runtime,
            matchesCurrentScope: () =>
            {
                predicateEntered.Set();
                releasePredicate.Wait();
                return true;
            }
        ));

        var resolution = Task.Run(registry.ResolveCurrentScope);
        predicateEntered.Wait();
        registration.Dispose();
        releasePredicate.Set();

        await Assert.That(await resolution).IsSameReferenceAs(runtime);
        await Assert.That(registry.ResolveCurrentScope()).IsNull();
    }

    [Test]
    public async Task AsyncLocalRouteMatchingIsIsolatedAcrossParallelTasks()
    {
        var registry = new AllureRuntimeRegistry();
        var activeRoute = new AsyncLocal<string?>();
        var runtimes = Enumerable.Range(0, 16)
            .ToDictionary(index => $"route-{index}", index => new TestRuntime($"runtime-{index}"));
        var registrations = runtimes.Select(pair => registry.Install(new TestRuntimeRoute(
            pair.Key,
            pair.Value,
            matchesCurrentScope: () => activeRoute.Value == pair.Key
        ))).ToArray();

        try
        {
            await Task.WhenAll(runtimes.Select(pair => Task.Run(async () =>
            {
                activeRoute.Value = pair.Key;
                for (var iteration = 0; iteration < 50; iteration++)
                {
                    await Task.Yield();
                    if (!ReferenceEquals(registry.ResolveCurrentScope(), pair.Value))
                    {
                        throw new InvalidOperationException($"Route isolation failed for {pair.Key}.");
                    }
                }
            })));
        }
        finally
        {
            foreach (var registration in registrations)
            {
                registration.Dispose();
            }
        }
    }
}
