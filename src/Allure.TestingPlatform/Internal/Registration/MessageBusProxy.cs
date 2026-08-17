using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.TestingPlatform.Sdk.Registration;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Messages;

namespace Allure.TestingPlatform.Internal.Registration;

sealed class MessageBusProxy : IMessageBus, IAllureTestingPlatformMessageChannel, IDisposable
{
    int disposed = 0;

    IMessageBus? target;

    public bool CanPublish => Volatile.Read(ref this.target) is not null;

    public Task PublishAsync(IDataProducer dataProducer, IData data)
    {
        this.EnsureNotDisposed();

        var target = Volatile.Read(ref this.target)
            ?? throw new InvalidOperationException(
                "No Microsoft Testing Platform request is currently active."
            );

        return target.PublishAsync(dataProducer, data);
    }

    public void SetTarget(IMessageBus messageBus)
    {
        this.EnsureNotDisposed();
        Volatile.Write(ref this.target, messageBus);
    }

    public void ClearTarget()
    {
        this.EnsureNotDisposed();
        Volatile.Write(ref this.target, null);
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
