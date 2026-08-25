using System;
using Allure.Model;

namespace Allure.Net.Commons;

/// <summary>
/// This class is a part of the legacy API compatibility layer and will be
/// removed in a future update.
/// Please, switch to <see cref="AllureInProcessApi"/>.
/// </summary>
[Obsolete("Use Allure.AllureInProcessApi instead.")]
public class AllureLifecycle
{
    public static AllureLifecycle Instance { get; } = new();

    /// <summary>
    /// Please, switch to <see cref="AllureInProcessApi.UpdateFixtureResult(Action{FixtureResult})"/>.
    /// If the callback produces the output data, use
    /// <see cref="AllureInProcessApi.ReadFixtureResult{TResult}(Func{FixtureResult, TResult})"/> instead.
    /// </summary>
    public AllureLifecycle UpdateFixture(Action<FixtureResult> update)
    {
        AllureInProcessApi.UpdateFixtureResult(update);
        return this;
    }

    /// <summary>
    /// Please, switch to <see cref="AllureInProcessApi.UpdateTestResult(Action{TestResult})"/>.
    /// If the callback produces the output data, use
    /// <see cref="AllureInProcessApi.ReadTestResult{TResult}(Func{TestResult, TResult})"/> instead.
    /// </summary>
    public AllureLifecycle UpdateTestCase(Action<TestResult> update)
    {
        AllureInProcessApi.UpdateTestResult(update);
        return this;
    }

    /// <summary>
    /// Please, switch to <see cref="AllureInProcessApi.UpdateStepResult(Action{StepResult})"/>.
    /// If the callback produces the output data, use
    /// <see cref="AllureInProcessApi.ReadStepResult{TResult}(Func{StepResult, TResult})"/> instead.
    /// </summary>
    public AllureLifecycle UpdateStep(Action<StepResult> update)
    {
        AllureInProcessApi.UpdateStepResult(update);
        return this;
    }
}