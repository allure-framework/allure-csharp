using System;

namespace Allure.Internal;

sealed class RuntimeRegistrationHandle(Action remove) : IDisposable
{
    public void Dispose()
    {
        remove();
    }
}
