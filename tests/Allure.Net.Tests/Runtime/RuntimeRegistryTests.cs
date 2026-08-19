using Allure.Net.Tests.Infrastructure;
using Allure.Runtime;

namespace Allure.Net.Tests.Runtime;

public class RuntimeRegistryTests
{
    [Test]
    public async Task ReturnsNullWhenNoRouteMatches()
    {
        var registry = new AllureRuntimeRegistry();

        await Assert.That(registry.ResolveCurrentScope()).IsNull();
        await Assert.That(registry.ResolveGlobalScope()).IsNull();
    }

    [Test]
    public async Task ResolvesMatchingCurrentRuntimeForCurrentScopeOnly()
    {
        var registry = new AllureRuntimeRegistry();
        var runtime = new TestRuntime("current");
        using var registration = registry.Install(
            new TestRuntimeRoute("current", runtime, matchesCurrentScope: () => true)
        );

        await Assert.That(registry.ResolveCurrentScope()).IsSameReferenceAs(runtime);
        await Assert.That(registry.ResolveGlobalScope()).IsNull();
    }

    [Test]
    public async Task ResolvesMatchingGlobalRuntime()
    {
        var registry = new AllureRuntimeRegistry();
        var runtime = new TestRuntime("global");
        using var registration = registry.Install(
            new TestRuntimeRoute("global", runtime, matchesGlobalScope: () => true)
        );

        await Assert.That(registry.ResolveGlobalScope()).IsSameReferenceAs(runtime);
        await Assert.That(registry.ResolveCurrentScope()).IsNull();
    }

    [Test]
    public async Task ResolvesMatchingCurrentAndGlobalRuntimeForCurrentAndGlobalScopes()
    {
        var registry = new AllureRuntimeRegistry();
        var runtime = new TestRuntime("current");
        using var registration = registry.Install(
            new TestRuntimeRoute(
                "current",
                runtime,
                matchesCurrentScope: () => true,
                matchesGlobalScope: () => true
            )
        );

        await Assert.That(registry.ResolveCurrentScope()).IsSameReferenceAs(runtime);
        await Assert.That(registry.ResolveGlobalScope()).IsSameReferenceAs(runtime);
    }

    [Test]
    public async Task GlobalScopeMultipleMatchesNarrowedByCurrentScope()
    {
        var registry = new AllureRuntimeRegistry();
        var current = new TestRuntime("currentAndGlobal");
        var global = new TestRuntime("global");
        using var currentRegistration = registry.Install(
            new TestRuntimeRoute(
                "currentAndGlobal",
                current,
                matchesCurrentScope: () => true,
                matchesGlobalScope: () => true
            )
        );
        using var globalRegistration = registry.Install(
            new TestRuntimeRoute("global", global, matchesGlobalScope: () => true)
        );

        await Assert.That(registry.ResolveCurrentScope()).IsSameReferenceAs(current);
        await Assert.That(registry.ResolveGlobalScope()).IsSameReferenceAs(current);
    }

    [Test]
    public async Task GlobalScopeTieBreakingAccountsForAvailibility()
    {
        var registry = new AllureRuntimeRegistry();
        var current = new TestRuntime("currentAndGlobal", isAvailable: false);
        var global = new TestRuntime("global");
        using var currentRegistration = registry.Install(
            new TestRuntimeRoute(
                "currentAndGlobal",
                current,
                matchesCurrentScope: () => true,
                matchesGlobalScope: () => true
            )
        );
        using var globalRegistration = registry.Install(
            new TestRuntimeRoute("global", global, matchesGlobalScope: () => true)
        );

        await Assert.That(registry.ResolveGlobalScope()).IsNull();
    }

    [Test]
    public async Task GlobalScopeDoesNotConsiderCurrentScopeForGlobalDisabledRoutes()
    {
        var registry = new AllureRuntimeRegistry();
        var current = new TestRuntime("currentAndGlobal");
        var global = new TestRuntime("global");
        using var currentRegistration = registry.Install(
            new TestRuntimeRoute(
                "currentAndGlobal",
                current,
                matchesCurrentScope: () => true
            )
        );
        using var globalRegistration = registry.Install(
            new TestRuntimeRoute("global", global, matchesGlobalScope: () => true)
        );

        await Assert.That(registry.ResolveGlobalScope()).IsSameReferenceAs(global);
    }

    [Test]
    public async Task DisabledRuntimeDoesNotResolve()
    {
        var registry = new AllureRuntimeRegistry();
        var runtime = new TestRuntime("disabled", isAvailable: false);
        using var registration = registry.Install(
            new TestRuntimeRoute(
                "disabled",
                runtime,
                matchesCurrentScope: () => true,
                matchesGlobalScope: () => true
            )
        );

        await Assert.That(registry.ResolveCurrentScope()).IsNull();
        await Assert.That(registry.ResolveGlobalScope()).IsNull();
    }

    [Test]
    public async Task ResolutionObservesRuntimeAvailabilityChanges()
    {
        var registry = new AllureRuntimeRegistry();
        var runtime = new TestRuntime("runtime", isAvailable: false);
        using var registration = registry.Install(
            new TestRuntimeRoute("runtime", runtime, matchesCurrentScope: () => true)
        );

        await Assert.That(registry.ResolveCurrentScope()).IsNull();

        runtime.IsAvailable = true;

        await Assert.That(registry.ResolveCurrentScope()).IsSameReferenceAs(runtime);
    }

    [Test]
    public async Task DominatingRouteSuppressesOtherMatches()
    {
        var registry = new AllureRuntimeRegistry();
        var winner = new TestRuntime("winner");
        using var winnerRegistration = registry.Install(
            new TestRuntimeRoute(
                "winner",
                winner,
                matchesCurrentScope: () => true,
                suppressedRouteIds: ["other"]
            )
        );
        using var otherRegistration = registry.Install(
            new TestRuntimeRoute(
                "other",
                new TestRuntime("other"),
                matchesCurrentScope: () => true
            )
        );

        await Assert.That(registry.ResolveCurrentScope()).IsSameReferenceAs(winner);
    }

    [Test]
    public async Task AmbiguousRoutesThrowWithRuntimeNamesAndIds()
    {
        var registry = new AllureRuntimeRegistry();
        using var first = registry.Install(
            new TestRuntimeRoute(
                "route-a",
                new TestRuntime("Runtime A"),
                matchesCurrentScope: () => true
            )
        );
        using var second = registry.Install(
            new TestRuntimeRoute(
                "route-b",
                new TestRuntime("Runtime B"),
                matchesCurrentScope: () => true
            )
        );

        await Assert.That(() => registry.ResolveCurrentScope())
            .Throws<InvalidOperationException>()
            .WithMessageContaining("Runtime A (route-a)")
            .And.WithMessageContaining("Runtime B (route-b)");
    }

    [Test]
    public async Task MutualSuppressionRemainsAmbiguous()
    {
        var registry = new AllureRuntimeRegistry();
        using var first = registry.Install(
            new TestRuntimeRoute(
                "a",
                new TestRuntime("A"),
                matchesCurrentScope: () => true,
                suppressedRouteIds: ["b"]
            )
        );
        using var second = registry.Install(
            new TestRuntimeRoute(
                "b",
                new TestRuntime("B"),
                matchesCurrentScope: () => true,
                suppressedRouteIds: ["a"]
            )
        );

        await Assert.That(() => registry.ResolveCurrentScope())
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task PartialSuppressionAmongThreeRoutesRemainsAmbiguous()
    {
        var registry = new AllureRuntimeRegistry();
        using var first = registry.Install(
            new TestRuntimeRoute(
                "a",
                new TestRuntime("A"),
                matchesCurrentScope: () => true,
                suppressedRouteIds: ["b"]
            )
        );
        using var second = registry.Install(
            new TestRuntimeRoute(
                "b",
                new TestRuntime("B"),
                matchesCurrentScope: () => true,
                suppressedRouteIds: ["c"]
            )
        );
        using var third = registry.Install(
            new TestRuntimeRoute(
                "c",
                new TestRuntime("C"),
                matchesCurrentScope: () => true
            )
        );

        await Assert.That(() => registry.ResolveCurrentScope())
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task RoutesWithSameIdCanBeExclusive()
    {
        var registry = new AllureRuntimeRegistry();
        var firstIsActive = true;
        var firstRuntime = new TestRuntime("first");
        var secondRuntime = new TestRuntime("second");
        using var first = registry.Install(
            new TestRuntimeRoute(
                "shared",
                firstRuntime,
                matchesCurrentScope: () => firstIsActive
            )
        );
        using var second = registry.Install(
            new TestRuntimeRoute(
                "shared",
                secondRuntime,
                matchesCurrentScope: () => !firstIsActive
            )
        );

        await Assert.That(registry.ResolveCurrentScope()).IsSameReferenceAs(firstRuntime);

        firstIsActive = false;

        await Assert.That(registry.ResolveCurrentScope()).IsSameReferenceAs(secondRuntime);
    }

    [Test]
    public async Task SimultaneouslyMatchingRoutesWithSameIdAreAmbiguous()
    {
        var registry = new AllureRuntimeRegistry();
        using var first = registry.Install(
            new TestRuntimeRoute(
                "shared",
                new TestRuntime("first"),
                matchesCurrentScope: () => true
            )
        );
        using var second = registry.Install(
            new TestRuntimeRoute(
                "shared",
                new TestRuntime("second"),
                matchesCurrentScope: () => true
            )
        );

        await Assert.That(() => registry.ResolveCurrentScope())
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task RejectsNullRoute()
    {
        var registry = new AllureRuntimeRegistry();

        await Assert.That(() => registry.Install(null!))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task RejectsInstallingSameRouteInstanceTwice()
    {
        var registry = new AllureRuntimeRegistry();
        var route = new TestRuntimeRoute("route", new TestRuntime("runtime"));
        using var registration = registry.Install(route);

        await Assert.That(() => registry.Install(route))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task DisposingRegistrationRemovesRoute()
    {
        var registry = new AllureRuntimeRegistry();
        var runtime = new TestRuntime("runtime");
        var registration = registry.Install(
            new TestRuntimeRoute("route", runtime, matchesCurrentScope: () => true)
        );

        await Assert.That(registry.ResolveCurrentScope()).IsSameReferenceAs(runtime);

        registration.Dispose();

        await Assert.That(registry.ResolveCurrentScope()).IsNull();
    }

    [Test]
    public async Task RegistrationDisposalIsIdempotent()
    {
        var registry = new AllureRuntimeRegistry();
        var registration = registry.Install(
            new TestRuntimeRoute(
                "route",
                new TestRuntime("runtime"),
                matchesCurrentScope: () => true
            )
        );

        registration.Dispose();
        registration.Dispose();

        await Assert.That(registry.ResolveCurrentScope()).IsNull();
    }

    [Test]
    public async Task OldRegistrationHandleDoesNotRemoveReinstalledRoute()
    {
        var registry = new AllureRuntimeRegistry();
        var runtime = new TestRuntime("runtime");
        var route = new TestRuntimeRoute(
            "route",
            runtime,
            matchesCurrentScope: () => true
        );
        var oldRegistration = registry.Install(route);
        oldRegistration.Dispose();
        using var newRegistration = registry.Install(route);

        oldRegistration.Dispose();

        await Assert.That(registry.ResolveCurrentScope()).IsSameReferenceAs(runtime);
    }
}
