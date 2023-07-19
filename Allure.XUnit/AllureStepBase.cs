using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Allure.Net.Commons;
using Allure.Net.Commons.Steps;

#if NETCOREAPP3_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

namespace Allure.Xunit
{
    public abstract class AllureStepBase<T> : IDisposable where T : AllureStepBase<T>
    {
        protected AllureStepBase() { }

        public void Dispose()
        {
#if NETCOREAPP3_0_OR_GREATER
            var failed = Marshal.GetExceptionPointers() != IntPtr.Zero;
#else
            var failed = Marshal.GetExceptionCode() != 0;
#endif
            if (failed)
            {
                CoreStepsHelper.FailStep();
            }
            else
            {
                CoreStepsHelper.PassStep();
            }
        }

        [Obsolete("For named parameters use NameAttribute; For skipped parameters use SkipAttribute")]
        public T SetParameter(string name, object value)
        {
            AllureLifecycle.Instance.UpdateStep(
                result =>
                {
                    result.parameters ??= new List<Parameter>();
                    result.parameters.Add(new Parameter { name = name, value = value?.ToString() });
                }
            );
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