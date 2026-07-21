namespace Allure.Abstractions;

public interface IAllureApiClient
{
    string Name { get; }

    IAllureApiEndpoint? ResolveCurrentScope();

    IAllureApiEndpoint? ResolveGlobalScope();
}
