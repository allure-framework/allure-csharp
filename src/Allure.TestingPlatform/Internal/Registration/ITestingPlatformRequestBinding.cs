using System;

namespace Allure.TestingPlatform.Internal.Registration;

/// <summary>
/// Represents the binding between a registered Allure Testing Platform instance
/// and a Microsoft Testing Platform request.
/// </summary>
/// <remarks>
/// Activating the binding makes the request's message bus available through the registration's
/// message channel. Releasing or disposing the binding makes that channel unavailable.
/// </remarks>
public interface ITestingPlatformRequestBinding : IDisposable
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
