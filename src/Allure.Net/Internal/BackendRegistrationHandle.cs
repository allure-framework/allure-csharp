using System;
using System.Threading;

namespace Allure.Internal;

sealed class RuntimeRegistrationHandle(Action remove) : IDisposable
{
    Action? callback = remove;

    public void Dispose()
    {
        Action? remove = Interlocked.Exchange(ref this.callback, null);
        remove?.Invoke();
    }
}
