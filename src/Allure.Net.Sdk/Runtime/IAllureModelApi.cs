using System;
using System.Collections.Immutable;
using Allure.Model;

namespace Allure.Sdk.Runtime;

/// <summary>
/// Reads and updates result objects in the current Allure execution state.
/// </summary>
public interface IAllureModelApi
{
    /// <summary>
    /// Updates the current result scope.
    /// </summary>
    void UpdateScope(Action<TestResultScope> update);

    /// <summary>
    /// Updates the result scope at the specified nesting level, where zero
    /// identifies the outermost scope.
    /// </summary>
    void UpdateScope(int level, Action<TestResultScope> update);

    /// <summary>
    /// Reads a value from the current result scope.
    /// </summary>
    TResult ReadScope<TResult>(Func<TestResultScope, TResult> read);

    /// <summary>
    /// Reads a value from the result scope at the specified nesting level,
    /// where zero identifies the outermost scope.
    /// </summary>
    TResult ReadScope<TResult>(int level, Func<TestResultScope, TResult> read);

    /// <summary>
    /// Updates every active result scope, from the current scope to the outermost.
    /// </summary>
    void UpdateAllScopes(Action<TestResultScope> update);

    /// <summary>
    /// Reads a value from every active result scope, from the current scope
    /// to the outermost.
    /// </summary>
    ImmutableArray<TResult> ReadAllScopes<TResult>(Func<TestResultScope, TResult> read);

    /// <summary>
    /// Updates the current fixture result.
    /// </summary>
    void UpdateFixtureResult(Action<FixtureResult> update);

    /// <summary>
    /// Reads a value from the current fixture result.
    /// </summary>
    TResult ReadFixtureResult<TResult>(Func<FixtureResult, TResult> read);

    /// <summary>
    /// Updates the current test result.
    /// </summary>
    void UpdateTestResult(Action<TestResult> update);

    /// <summary>
    /// Reads a value from the current test result.
    /// </summary>
    TResult ReadTestResult<TResult>(Func<TestResult, TResult> read);

    /// <summary>
    /// Updates the current step result.
    /// </summary>
    void UpdateStepResult(Action<StepResult> update);

    /// <summary>
    /// Updates the step result at the specified nesting level, where zero
    /// identifies the outermost step.
    /// </summary>
    void UpdateStepResult(int level, Action<StepResult> update);

    /// <summary>
    /// Reads a value from the current step result.
    /// </summary>
    TResult ReadStepResult<TResult>(Func<StepResult, TResult> read);

    /// <summary>
    /// Reads a value from the step result at the specified nesting level,
    /// where zero identifies the outermost step.
    /// </summary>
    TResult ReadStepResult<TResult>(int level, Func<StepResult, TResult> read);

    /// <summary>
    /// Updates every active step result, from the current step to the outermost.
    /// </summary>
    void UpdateAllStepResults(Action<StepResult> update);

    /// <summary>
    /// Reads a value from every active step result, from the current step
    /// to the outermost.
    /// </summary>
    ImmutableArray<TResult> ReadAllSteps<TResult>(Func<StepResult, TResult> read);

    /// <summary>
    /// Updates the current step, fixture, or test result.
    /// </summary>
    void UpdateCurrentExecutableItem(Action<ExecutableItem> update);

    /// <summary>
    /// Reads a value from the current step, fixture, or test result.
    /// </summary>
    TResult ReadCurrentExecutableItem<TResult>(Func<ExecutableItem, TResult> read);
}
