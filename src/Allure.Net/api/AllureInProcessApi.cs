using System;
using System.Diagnostics.CodeAnalysis;
using Allure.Model;
using Allure.Runtime;

namespace Allure;

public static class AllureInProcessApi
{
    public static void UpdateTestResult(Action<TestResult> update)
    {
        AllureFrontend.InProcessApi.UpdateTestResult(update);
    }

    public static bool TryReadTestResult<TResult>(
        Func<TestResult, TResult> read,
        [MaybeNullWhen(false)] out TResult result
    ) =>
        AllureFrontend.InProcessApi.TryReadTestResult(read, out result);

    public static TResult ReadTestResult<TResult>(
        Func<TestResult, TResult> read
    ) =>
        TryReadTestResult(read, out var result)
            ? result
            : throw new InvalidOperationException(
                "Cannot read test result: no test is currently running."
            );

    public static TResult ReadTestResult<TResult>(
        Func<TestResult, TResult> read,
        TResult fallback
    ) =>
        TryReadTestResult(read, out var result)
            ? result
            : fallback;

    public static TResult ReadTestResult<TResult>(
        Func<TestResult, TResult> read,
        Func<TResult> fallbackFactory
    ) =>
        TryReadTestResult(read, out var result)
            ? result
            : fallbackFactory();

    public static void UpdateFixtureResult(Action<FixtureResult> update)
    {
        AllureFrontend.InProcessApi.UpdateFixtureResult(update);
    }

    public static bool TryReadFixtureResult<TResult>(
        Func<FixtureResult, TResult> read,
        [MaybeNullWhen(false)] out TResult result
    ) =>
        AllureFrontend.InProcessApi.TryReadFixtureResult(read, out result);

    public static TResult ReadFixtureResult<TResult>(
        Func<FixtureResult, TResult> read
    ) =>
        TryReadFixtureResult(read, out var result)
            ? result
            : throw new InvalidOperationException(
                "Cannot read fixture result: no fixture is currently running."
            );

    public static TResult ReadFixtureResult<TResult>(
        Func<FixtureResult, TResult> read,
        TResult fallback
    ) =>
        TryReadFixtureResult(read, out var result)
            ? result
            : fallback;

    public static TResult ReadFixtureResult<TResult>(
        Func<FixtureResult, TResult> read,
        Func<TResult> fallbackFactory
    ) =>
        TryReadFixtureResult(read, out var result)
            ? result
            : fallbackFactory();

    public static void UpdateStepResult(Action<StepResult> update)
    {
        AllureFrontend.InProcessApi.UpdateStepResult(update);
    }

    public static bool TryReadStepResult<TResult>(
        Func<StepResult, TResult> read,
        [MaybeNullWhen(false)] out TResult result
    ) =>
        AllureFrontend.InProcessApi.TryReadStepResult(read, out result);


    public static TResult ReadStepResult<TResult>(
        Func<StepResult, TResult> read
    ) =>
        TryReadStepResult(read, out var result)
            ? result
            : throw new InvalidOperationException(
                "Cannot read step result: no step is currently running."
            );


    public static TResult ReadStepResult<TResult>(
        Func<StepResult, TResult> read,
        TResult fallback
    ) =>
        TryReadStepResult(read, out var result)
            ? result
            : fallback;

    public static TResult ReadStepResult<TResult>(
        Func<StepResult, TResult> read,
        Func<TResult> fallbackFactory
    ) =>
        TryReadStepResult(read, out var result)
            ? result
            : fallbackFactory();

}
