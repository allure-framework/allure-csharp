using System;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Provides a constructed Allure runtime and owns its registration.
/// </summary>
/// <remarks>
/// Disposing this object cancels the registration and disposes the runtime
/// if it implements <see cref="IDisposable"/> or <see cref="IAsyncDisposable"/>.
/// </remarks>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public interface IAllureRuntimeRegistration<TRuntime> :
    IDisposable,
    IAsyncDisposable

    where TRuntime : IAllureRuntime
{
    /// <summary>
    /// Gets the constructed runtime.
    /// </summary>
    TRuntime Runtime { get; }
}
