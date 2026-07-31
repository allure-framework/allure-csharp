using System;
using System.Threading;
using System.Threading.Tasks;
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
/// <param name="runtime">The constructed runtime.</param>
/// <param name="routeRegistration">
/// The endpoint route registration, or <see langword="null"/> when no endpoint
/// was registered.
/// </param>
public sealed class AllureRuntimeRegistration<TRuntime>(
    TRuntime runtime,
    IDisposable? routeRegistration
) : IDisposable, IAsyncDisposable
    where TRuntime : IAllureRuntime
{
    int disposed = 0;

    /// <summary>
    /// Gets the constructed runtime.
    /// </summary>
    public TRuntime Runtime => runtime;

    /// <summary>
    /// Cancels the registration and synchronously disposes the runtime if it
    /// implements <see cref="IDisposable"/>.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref this.disposed, 1) != 0)
        {
            return;
        }

        var registration = Interlocked.Exchange(ref routeRegistration, null);

        try
        {
            registration?.Dispose();
        }
        finally
        {
            (this.Runtime as IDisposable)?.Dispose();
        }
    }

    /// <summary>
    /// Cancels the registration and disposes the runtime asynchronously when
    /// supported, falling back to synchronous disposal.
    /// </summary>
    /// <returns>A task that represents the asynchronous disposal operation.</returns>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref this.disposed, 1) != 0)
        {
            return;
        }

        var registration = Interlocked.Exchange(ref routeRegistration, null);

        try
        {
            registration?.Dispose();
        }
        finally
        {
            if (this.Runtime is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            }
            else
            {
                (this.Runtime as IDisposable)?.Dispose();
            }
        }
    }
}
