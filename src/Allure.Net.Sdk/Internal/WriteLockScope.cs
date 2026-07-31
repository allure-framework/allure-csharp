using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Allure.Sdk.Internal;

readonly struct WriteLockScope : IDisposable
{
    readonly ReaderWriterLockSlim rwLock;

    internal WriteLockScope(ReaderWriterLockSlim rwLock)
    {
        this.rwLock = rwLock;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        this.rwLock.ExitWriteLock();
    }
}
