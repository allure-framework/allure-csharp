using System;

namespace Allure.TestingPlatform.Internal.Runtime;

/// <summary>
/// Represents the binding between the Allure runtime and a Microsoft Testing Platform request.
/// </summary>
public interface IRequestRuntimeBinding : IDisposable
{
    /// <summary>
    /// Activates the request binding so that messages can be published.
    /// </summary>
    void Activate();

    /// <summary>
    /// Releases the active request binding.
    /// </summary>
    void Release();
}
