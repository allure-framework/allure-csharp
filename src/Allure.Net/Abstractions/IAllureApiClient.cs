namespace Allure.Abstractions;

public interface IAllureApiClient
{
    string Name { get; }

    bool IsAvailableInCurrentScope { get; }

    bool IsAvailableInGlobalScope { get; }

    IAllureApiEndpoint? ResolveCurrentScope();

    IAllureApiEndpoint? ResolveGlobalScope();
}
