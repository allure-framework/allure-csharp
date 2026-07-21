using Allure.Abstractions;

namespace Allure.Net.Tests.Infrastructure;

sealed class TestApiClient(
    string name,
    Func<IAllureApiEndpoint?>? currentScope = null,
    Func<IAllureApiEndpoint?>? globalScope = null
) : IAllureApiClient
{
    public string Name { get; } = name;

    public int CurrentScopeResolutionCount { get; private set; }

    public int GlobalScopeResolutionCount { get; private set; }

    public IAllureApiEndpoint? ResolveCurrentScope()
    {
        this.CurrentScopeResolutionCount++;
        return currentScope?.Invoke();
    }

    public IAllureApiEndpoint? ResolveGlobalScope()
    {
        this.GlobalScopeResolutionCount++;
        return globalScope?.Invoke();
    }
}
