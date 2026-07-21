using System.Runtime.CompilerServices;
using Allure.Abstractions;
using Allure.Runtime;

namespace Allure.Net.Tests.Infrastructure;

static class FacadeTestEnvironment
{
    static readonly AsyncLocal<Scope?> currentScope = new();

    [ModuleInitializer]
    internal static void Initialize() =>
        AllureFrontend.PrepareClient(new ScopedApiClient());

    public static Scope Use(
        IAllureApiEndpoint? current = null,
        IAllureApiEndpoint? global = null
    )
    {
        var scope = new Scope(current, global);
        currentScope.Value = scope;
        return scope;
    }

    public sealed class Scope(
        IAllureApiEndpoint? currentEndpoint,
        IAllureApiEndpoint? globalEndpoint
    ) : IDisposable
    {
        public IAllureApiEndpoint? CurrentEndpoint { get; } = currentEndpoint;

        public IAllureApiEndpoint? GlobalEndpoint { get; } = globalEndpoint;

        public int CurrentResolutionCount { get; set; }

        public int GlobalResolutionCount { get; set; }

        public void Dispose() => currentScope.Value = null;
    }

    sealed class ScopedApiClient : IAllureApiClient
    {
        public string Name => "facade test client";

        public IAllureApiEndpoint? ResolveCurrentScope()
        {
            var scope = currentScope.Value;
            if (scope is not null)
            {
                scope.CurrentResolutionCount++;
            }
            return scope?.CurrentEndpoint;
        }

        public IAllureApiEndpoint? ResolveGlobalScope()
        {
            var scope = currentScope.Value;
            if (scope is not null)
            {
                scope.GlobalResolutionCount++;
            }
            return scope?.GlobalEndpoint;
        }
    }
}
