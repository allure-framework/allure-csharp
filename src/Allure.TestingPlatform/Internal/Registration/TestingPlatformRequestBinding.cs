using System;
using System.Threading;

namespace Allure.TestingPlatform.Internal.Registration;

sealed class TestingPlatformRequestBinding(
    IAllureTestingPlatformRequestCoordinator coordinator,
    IServiceProvider serviceProvider
) :
    ITestingPlatformRequestBinding
{
    int released = 0;
    int disposed = 0;

    public IServiceProvider ServiceProvider => serviceProvider;

    public void Activate()
    {
        this.EnsureNotDisposed();

        if (Volatile.Read(ref this.released) != 0)
        {
            throw new InvalidOperationException(
                "The request runtime binding has already been released."
            );
        }

        coordinator.ActivateRequest(this);
    }

    public void Release()
    {
        if (Interlocked.Exchange(ref this.released, 1) == 0)
        {
            coordinator.ReleaseRequest(this);
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref this.disposed, 1) != 0)
        {
            return;
        }

        this.Release();
        coordinator.DisposeRequestBinding(this);
    }

    void EnsureNotDisposed()
    {
        if (Volatile.Read(ref this.disposed) != 0)
        {
            throw new ObjectDisposedException(this.GetType().FullName);
        }
    }
}
