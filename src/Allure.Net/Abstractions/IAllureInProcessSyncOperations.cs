using System;
using System.Diagnostics.CodeAnalysis;
using Allure.Model;

namespace Allure.Abstractions;

/// <summary>
/// Defines synchronous operations that require an in-process Allure runtime.
/// </summary>
public interface IAllureInProcessSyncOperations :
    IAllureSyncOperations<IAllureInProcessSyncStepContext, IAllureInProcessSyncFixtureContext>
{
    /// <summary>
    /// Updates the current test result, if one exists.
    /// </summary>
    void UpdateTestResult(Action<TestResult> update);

    /// <summary>
    /// Attempts to read a value from the current test result.
    /// </summary>
    bool TryReadTestResult<TResult>(
        Func<TestResult, TResult> read,
        [MaybeNullWhen(false)] out TResult result
    );

    /// <summary>
    /// Updates the current fixture result, if one exists.
    /// </summary>
    void UpdateFixtureResult(Action<FixtureResult> update);

    /// <summary>
    /// Attempts to read a value from the current fixture result.
    /// </summary>
    bool TryReadFixtureResult<TResult>(
        Func<FixtureResult, TResult> read,
        [MaybeNullWhen(false)] out TResult result
    );

    /// <summary>
    /// Updates the current step result, if one exists.
    /// </summary>
    void UpdateStepResult(Action<StepResult> update);

    /// <summary>
    /// Attempts to read a value from the current step result.
    /// </summary>
    bool TryReadStepResult<TResult>(
        Func<StepResult, TResult> read,
        [MaybeNullWhen(false)] out TResult result
    );
}
