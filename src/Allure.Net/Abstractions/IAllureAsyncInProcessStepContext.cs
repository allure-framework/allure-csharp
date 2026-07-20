using System;
using System.Diagnostics.CodeAnalysis;
using Allure.Model;

namespace Allure.Abstractions;


public interface IAllureAsyncInProcessStepContext : IAllureAsyncStepContext
{
    void UpdateStepResult(Action<StepResult> update);

    bool TryReadStepResult<TResult>(Func<StepResult, TResult> read, [MaybeNullWhen(false)] out TResult value);
}
