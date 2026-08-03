using System.Runtime.CompilerServices;
using System.Threading;

namespace Allure.Sdk.Internal;

static class ReaderWriterLockSlimExtensions
{
    extension (ReaderWriterLockSlim rwLock)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadLockScope EnterReadScope()
        {
            rwLock.EnterReadLock();
            return new ReadLockScope(rwLock);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WriteLockScope EnterWriteScope()
        {
            rwLock.EnterWriteLock();
            return new WriteLockScope(rwLock);
        }
    }
}
