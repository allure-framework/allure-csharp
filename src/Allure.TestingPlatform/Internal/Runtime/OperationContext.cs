using System;
using Allure.Abstractions;
using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Internal.Runtime;

abstract class OperationContext(
    IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> runtime
) : IAllureOperationContext, IDisposable
{
    bool disposed = false;

    protected IAllureRuntimeBase Runtime => runtime;

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
