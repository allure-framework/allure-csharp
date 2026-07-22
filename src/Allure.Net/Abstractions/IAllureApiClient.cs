namespace Allure.Abstractions;

/// <summary>
/// Resolves API endpoints for calls made by test code.
/// </summary>
public interface IAllureApiClient
{
    /// <summary>
    /// Gets the client name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Resolves the endpoint associated with the current test or fixture scope.
    /// </summary>
    IAllureApiEndpoint? ResolveCurrentScope();

    /// <summary>
    /// Resolves an endpoint capable of accepting global result data.
    /// </summary>
    IAllureApiEndpoint? ResolveGlobalScope();
}
