using System;
using System.Diagnostics.CodeAnalysis;
using Allure.Abstractions;
using Allure.Model;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Runtime;

abstract class StepOperationContext(IAllureRuntime runtime, int level) :
    OperationContext(runtime),
    IAllureOperationContext
{
    protected int Level => level;

    public bool TryReadStepResult<T>(
        Func<StepResult, T> read,
        [MaybeNullWhen(false)] out T result
    )
    {
        this.EnsureInScope();

        if (this.CurrentState.HasStep)
        {
            result = this.Runtime.ModelApi.ReadStepResult(level, read);
            return true;
        }

        result = default;
        return false;
    }

    public void UpdateStepResult(Action<StepResult> update)
    {
        this.EnsureInScope();

        this.Runtime.ModelApi.UpdateStepResult(level, update);
    }

    protected override string ScopingErrorMessage =>
        "The step associated with this context has already finished.";
}
