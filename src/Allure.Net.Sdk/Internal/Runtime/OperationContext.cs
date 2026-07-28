using System;
using Allure.Abstractions;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Runtime;

abstract class OperationContext(IAllureRuntime runtime) : IAllureOperationContext, IDisposable
{
    bool disposed = false;

    protected IAllureRuntime Runtime => runtime;

    protected AllureExecutionState CurrentState => runtime.ContextApi.CurrentState;

    protected abstract string ScopingErrorMessage { get; }

    public IAllureParameterSerializer ParameterSerializer => runtime.ParameterSerializer;

    public void Dispose()
    {
        this.disposed = true;
    }

    protected void EnsureInScope()
    {
        if (this.disposed)
        {
            throw new InvalidOperationException(
                this.ScopingErrorMessage
            );
        }
    }
}
