using System;
using System.Threading;
using Microsoft.Testing.Platform.Messages;

namespace Allure.TestingPlatform.Internal.Registration;

sealed class ServiceProviderProxy(MessageBusProxy messageBusProxy) :
    IServiceProvider,
    IDisposable
{
    IServiceProvider? target;

    int disposed = 0;

    public void SetTarget(IServiceProvider serviceProvider)
    {
        this.EnsureNotDisposed();
        Volatile.Write(ref this.target, serviceProvider);
    }

    internal void ClearTarget()
    {
        this.EnsureNotDisposed();
        Volatile.Write(ref this.target, null);
    }

    internal bool IsBoundTo(IServiceProvider provider)
    {
        this.EnsureNotDisposed();

        return ReferenceEquals(Volatile.Read(ref this.target), provider);
    }

    public object? GetService(Type serviceType)
    {
        this.EnsureNotDisposed();

        if (serviceType == typeof (IMessageBus))
        {
            return messageBusProxy;
        }

        var provider = Volatile.Read(ref this.target)
            ?? throw new InvalidOperationException(
                "The service provider proxy has not been bound to its target."
            );

        return provider.GetService(serviceType);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref this.disposed, 1) == 0)
        {
            Volatile.Write(ref this.target, null);
        }
    }

    void EnsureNotDisposed()
    {
        if (Volatile.Read(ref this.disposed) != 0)
        {
            throw new ObjectDisposedException(this.GetType().FullName);
        }
    }
}
