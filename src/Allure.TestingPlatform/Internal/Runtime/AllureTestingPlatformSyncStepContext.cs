using System;
using System.Diagnostics.CodeAnalysis;
using Allure.Abstractions;
using Allure.Model;

namespace Allure.TestingPlatform.Internal.Runtime;

sealed class AllureTestingPlatformSyncStepContext(
    IAllureInProcessAsyncStepContext asyncContext
) :
    IAllureInProcessSyncStepContext
{
    public IAllureParameterSerializer ParameterSerializer => asyncContext.ParameterSerializer;

    public void AddParameter(Parameter parameter) =>
        asyncContext.AddParameterAsync(parameter)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void SetName(string newName) =>
        asyncContext.SetNameAsync(newName)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public bool TryReadStepResult<TResult>(
        Func<StepResult, TResult> read,
        [MaybeNullWhen(false)] out TResult value
    ) =>
        asyncContext.TryReadStepResult(read, out value);

    public void UpdateStepResult(Action<StepResult> update) =>
        asyncContext.UpdateStepResult(update);
}
