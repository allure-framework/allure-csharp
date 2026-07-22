using System;
using System.Diagnostics.CodeAnalysis;
using Allure.Model;

namespace Allure.Abstractions;


/// <summary>
/// Provides direct access to a step result from an asynchronous operation.
/// </summary>
public interface IAllureAsyncInProcessStepContext : IAllureAsyncStepContext
{
    /// <summary>
    /// Updates the current step result.
    /// </summary>
    void UpdateStepResult(Action<StepResult> update);

    /// <summary>
    /// Attempts to read a value from the current step result.
    /// </summary>
    bool TryReadStepResult<TResult>(
        Func<StepResult, TResult> read,
        [MaybeNullWhen(false)] out TResult value
    );
}
