using System;
using System.Diagnostics.CodeAnalysis;
using Allure.Model;
using Allure.Runtime;

namespace Allure;

/// <summary>
/// Provides direct access to Allure model objects when the API endpoint runs in the current process.
/// </summary>
/// <remarks>
/// These operations are unavailable for out-of-process Allure integrations.
/// </remarks>
public static class AllureInProcessApi
{
    /// <summary>
    /// Updates the current test result, if one exists.
    /// </summary>
    public static void UpdateTestResult(Action<TestResult> update)
    {
        AllureFrontend.InProcessApi.UpdateTestResult(update);
    }

    /// <summary>
    /// Attempts to read a value from the current test result.
    /// </summary>
    public static bool TryReadTestResult<TResult>(
        Func<TestResult, TResult> read,
        [MaybeNullWhen(false)] out TResult result
    ) =>
        AllureFrontend.InProcessApi.TryReadTestResult(read, out result);

    /// <summary>
    /// Reads a value from the current test result.
    /// </summary>
    /// <exception cref="InvalidOperationException">No test is currently running.</exception>
    public static TResult ReadTestResult<TResult>(
        Func<TestResult, TResult> read
    ) =>
        TryReadTestResult(read, out var result)
            ? result
            : throw new InvalidOperationException(
                "Cannot read test result: no test is currently running."
            );

    /// <summary>
    /// Reads a value from the current test result, or returns a fallback value.
    /// </summary>
    public static TResult ReadTestResult<TResult>(
        Func<TestResult, TResult> read,
        TResult fallback
    ) =>
        TryReadTestResult(read, out var result)
            ? result
            : fallback;

    /// <summary>
    /// Reads a value from the current test result, or creates a fallback value.
    /// </summary>
    public static TResult ReadTestResult<TResult>(
        Func<TestResult, TResult> read,
        Func<TResult> fallbackFactory
    ) =>
        TryReadTestResult(read, out var result)
            ? result
            : fallbackFactory();

    /// <summary>
    /// Updates the current fixture result, if one exists.
    /// </summary>
    public static void UpdateFixtureResult(Action<FixtureResult> update)
    {
        AllureFrontend.InProcessApi.UpdateFixtureResult(update);
    }

    /// <summary>
    /// Attempts to read a value from the current fixture result.
    /// </summary>
    public static bool TryReadFixtureResult<TResult>(
        Func<FixtureResult, TResult> read,
        [MaybeNullWhen(false)] out TResult result
    ) =>
        AllureFrontend.InProcessApi.TryReadFixtureResult(read, out result);

    /// <summary>
    /// Reads a value from the current fixture result.
    /// </summary>
    /// <exception cref="InvalidOperationException">No fixture is currently running.</exception>
    public static TResult ReadFixtureResult<TResult>(
        Func<FixtureResult, TResult> read
    ) =>
        TryReadFixtureResult(read, out var result)
            ? result
            : throw new InvalidOperationException(
                "Cannot read fixture result: no fixture is currently running."
            );

    /// <summary>
    /// Reads a value from the current fixture result, or returns a fallback value.
    /// </summary>
    public static TResult ReadFixtureResult<TResult>(
        Func<FixtureResult, TResult> read,
        TResult fallback
    ) =>
        TryReadFixtureResult(read, out var result)
            ? result
            : fallback;

    /// <summary>
    /// Reads a value from the current fixture result, or creates a fallback value.
    /// </summary>
    public static TResult ReadFixtureResult<TResult>(
        Func<FixtureResult, TResult> read,
        Func<TResult> fallbackFactory
    ) =>
        TryReadFixtureResult(read, out var result)
            ? result
            : fallbackFactory();

    /// <summary>
    /// Updates the current step result, if one exists.
    /// </summary>
    public static void UpdateStepResult(Action<StepResult> update)
    {
        AllureFrontend.InProcessApi.UpdateStepResult(update);
    }

    /// <summary>
    /// Attempts to read a value from the current step result.
    /// </summary>
    public static bool TryReadStepResult<TResult>(
        Func<StepResult, TResult> read,
        [MaybeNullWhen(false)] out TResult result
    ) =>
        AllureFrontend.InProcessApi.TryReadStepResult(read, out result);


    /// <summary>
    /// Reads a value from the current step result.
    /// </summary>
    /// <exception cref="InvalidOperationException">No step is currently running.</exception>
    public static TResult ReadStepResult<TResult>(
        Func<StepResult, TResult> read
    ) =>
        TryReadStepResult(read, out var result)
            ? result
            : throw new InvalidOperationException(
                "Cannot read step result: no step is currently running."
            );


    /// <summary>
    /// Reads a value from the current step result, or returns a fallback value.
    /// </summary>
    public static TResult ReadStepResult<TResult>(
        Func<StepResult, TResult> read,
        TResult fallback
    ) =>
        TryReadStepResult(read, out var result)
            ? result
            : fallback;

    /// <summary>
    /// Reads a value from the current step result, or creates a fallback value.
    /// </summary>
    public static TResult ReadStepResult<TResult>(
        Func<StepResult, TResult> read,
        Func<TResult> fallbackFactory
    ) =>
        TryReadStepResult(read, out var result)
            ? result
            : fallbackFactory();

}
