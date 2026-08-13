using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

sealed class AllureRuntimeRegistration<TRuntime>(
    TRuntime runtime,
    IDisposable? routeRegistration
) :
    IAllureRuntimeRegistration<TRuntime>
    where TRuntime : IAllureRuntimeBase
{
    int disposed = 0;

    public TRuntime Runtime => runtime;

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
