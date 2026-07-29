using System.Collections.Immutable;
using Allure.Abstractions;
using Allure.Runtime;

namespace Allure.Net.Tests.Infrastructure;

static class FacadeTestEnvironment
{
    static readonly AsyncLocal<Scope?> currentScope = new();

    public static Scope Use(
        IAllureRuntimeEndpoint? current = null,
        IAllureRuntimeEndpoint? global = null
    )
    {
        var scope = new Scope(currentScope.Value, current, global);
        currentScope.Value = scope;
        scope.Install();
        return scope;
    }

    public sealed class Scope(
        Scope? previous,
        IAllureRuntimeEndpoint? currentEndpoint,
        IAllureRuntimeEndpoint? globalEndpoint
    ) : IDisposable
    {
        readonly List<IDisposable> registrations = [];

        public IAllureRuntimeEndpoint? CurrentEndpoint { get; } = currentEndpoint;

        public IAllureRuntimeEndpoint? GlobalEndpoint { get; } = globalEndpoint;

        public int CurrentResolutionCount { get; set; }

        public int GlobalResolutionCount { get; set; }

        internal void Install()
        {
            if (this.CurrentEndpoint is not null)
            {
                this.registrations.Add(AllureRuntimeRouter.Install(
                    new ScopedRoute(this, this.CurrentEndpoint, current: true)
                ));
            }
            if (this.GlobalEndpoint is not null)
            {
                this.registrations.Add(AllureRuntimeRouter.Install(
                    new ScopedRoute(this, this.GlobalEndpoint, current: false)
                ));
            }
        }

        public void Dispose()
        {
            for (var index = this.registrations.Count - 1; index >= 0; index--)
            {
                this.registrations[index].Dispose();
            }
            if (ReferenceEquals(currentScope.Value, this))
            {
                currentScope.Value = previous;
            }
        }
    }

    sealed class ScopedRoute(
        Scope scope,
        IAllureRuntimeEndpoint endpoint,
        bool current
    ) : IAllureRuntimeRoute
    {
        public string Id { get; } = $"test-{Guid.NewGuid():N}";

        public bool MatchesCurrentScope
        {
            get
            {
                var matches = current && ReferenceEquals(currentScope.Value, scope);
                if (matches)
                {
                    scope.CurrentResolutionCount++;
                }
                return matches;
            }
        }

        public bool MatchesGlobalScope
        {
            get
            {
                var matches = !current && ReferenceEquals(currentScope.Value, scope);
                if (matches)
                {
                    scope.GlobalResolutionCount++;
                }
                return matches;
            }
        }

        public ImmutableHashSet<string> SuppressedRouteIds { get; } = [];

        public IAllureRuntimeEndpoint Endpoint { get; } = endpoint;
    }
}
