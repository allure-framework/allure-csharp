using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Runtime;

class RuntimeAsyncStepContext(IAllureRuntime runtime, int level) :
    IAllureInProcessAsyncStepContext
{
    AllureExecutionState CurrentState => runtime.ContextApi.CurrentState;

    IAllureParameterSerializer IAllureOperationContext.ParameterSerializer => runtime.ParameterSerializer;

    public Task AddParameterAsync(Parameter parameter, CancellationToken _)
    {
        runtime.ModelApi.UpdateStepResult(
            level,
            (stepResult) => stepResult.Parameters.Add(parameter)
        );
        return Task.CompletedTask;
    }

    public Task SetNameAsync(string newName, CancellationToken _)
    {
        runtime.ModelApi.UpdateStepResult(
            level,
            (stepResult) => stepResult.Name = newName
        );
        return Task.CompletedTask;
    }

    public bool TryReadStepResult<T>(
        Func<StepResult, T> read,
        [MaybeNullWhen(false)] out T result
    )
    {
        if (this.CurrentState.HasStep)
        {
            result = runtime.ModelApi.ReadStepResult(level, read);
            return true;
        }

        result = default;
        return false;
    }

    public void UpdateStepResult(Action<StepResult> update)
    {
        if (this.CurrentState.HasStep)
        {
            runtime.ModelApi.UpdateStepResult(level, update);
        }
    }

    Task AddParameter(Parameter parameter)
    {
        runtime.ModelApi.UpdateStepResult(
            level,
            (stepResult) => stepResult.Parameters.Add(parameter)
        );
        return Task.CompletedTask;
    }
}
