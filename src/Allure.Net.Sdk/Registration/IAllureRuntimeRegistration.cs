using System;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Provides a constructed Allure runtime and owns its registration.
/// </summary>
/// <remarks>
/// <see cref="IDisposable.Dispose"/> removes the in-process endpoint, if one was
/// installed, and disposes a runtime that implements <see cref="IDisposable"/>.
/// <see cref="IAsyncDisposable.DisposeAsync"/> removes the endpoint and prefers
/// <see cref="IAsyncDisposable"/> when the runtime supports it, falling back to
/// <see cref="IDisposable"/>.
/// </remarks>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public interface IAllureRuntimeRegistration<out TRuntime> :
    IDisposable,
    IAsyncDisposable

    where TRuntime : IAllureRuntimeBase
{
    /// <summary>
    /// Gets the constructed runtime.
    /// </summary>
    TRuntime Runtime { get; }
}
