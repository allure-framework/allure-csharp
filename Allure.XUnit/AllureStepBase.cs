using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Allure.Net.Commons;

#if NETCOREAPP3_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

namespace Allure.Xunit
{
    public abstract class AllureStepBase<T> : IDisposable where T : AllureStepBase<T>
    {
        protected AllureStepBase(string uuid)
        {
            UUID = uuid;
        }

        private string UUID { get; }

        public void Dispose()
        {
#if NETCOREAPP3_0_OR_GREATER
            var failed = Marshal.GetExceptionPointers() != IntPtr.Zero;
#else
            var failed = Marshal.GetExceptionCode() != 0;
#endif
            if (failed)
            {
                if (this is AllureBefore || this is AllureAfter)
                {
                    Steps.StopFixtureSuppressTestCase(result => result.status = Status.failed);
                }
                else
                {
                    Steps.FailStep(UUID);
                }
            }
            else
            {
                if (this is AllureBefore || this is AllureAfter)
                {
                    Steps.StopFixtureSuppressTestCase(result => result.status = Status.passed);
                }
                else
                {
                    Steps.PassStep(UUID);
                }
            }
        }

        [Obsolete("For named parameters use NameAttribute; For skipped parameters use SkipAttribute")]
        public T SetParameter(string name, object value)
        {
            AllureLifecycle.Instance.UpdateStep(UUID,
                result =>
                {
                    result.parameters ??= new List<Parameter>();
                    result.parameters.Add(new Parameter { name = name, value = value?.ToString() });
                });
            return (T) this;
        }

#if NETCOREAPP3_0_OR_GREATER
        [Obsolete("For named parameters use NameAttribute; For skipped parameters use SkipAttribute")]
        public T SetParameter(object value, [CallerArgumentExpression("value")] string name = null)
        {
            return SetParameter(name, value);
        }
#endif
    }
}