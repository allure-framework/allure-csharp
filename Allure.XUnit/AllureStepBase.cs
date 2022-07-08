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
        protected AllureStepBase(ExecutableItem executableItem)
        {
            ExecutableItem = executableItem;
        }

        private ExecutableItem ExecutableItem { get; }

        public void Dispose()
        {
#if NETCOREAPP3_0_OR_GREATER
            var failed = Marshal.GetExceptionPointers() != IntPtr.Zero;
#else
            var failed = Marshal.GetExceptionCode() != 0;
#endif
            if (failed)
                Steps.FailStep(ExecutableItem);
            else
                Steps.PassStep(ExecutableItem);
        }

        public T SetParameter(string name, object value)
        {
            var parameters = ExecutableItem.parameters ??= new List<Parameter>();
            parameters.Add(new Parameter {name = name, value = value?.ToString()});
            return (T) this;
        }

#if NETCOREAPP3_0_OR_GREATER
        public T SetParameter(object value, [CallerArgumentExpression("value")] string name = null)
        {
            return SetParameter(name, value);
        }
#endif
    }
}