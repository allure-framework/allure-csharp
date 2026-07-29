using System;
using Allure.Model;

namespace Allure.Net.Commons;

/// <summary>
/// This class is a part of the legacy API and will be removed in a future update.
/// Please, switch to <see cref="AllureInProcessApi"/>.
/// </summary>
[Obsolete("Use Allure.AllureInProcessApi instead.")]
public class AllureLifecycle
{
    public static AllureLifecycle Instance { get; } = new();

    /// <summary>
    /// Please, switch to <see cref="AllureInProcessApi.UpdateFixtureResult(Action{FixtureResult})"/>.
    /// </summary>
    public AllureLifecycle UpdateFixture(Action<FixtureResult> update)
    {
        AllureInProcessApi.UpdateFixtureResult(update);
        return this;
    }

    /// <summary>
    /// Please, switch to <see cref="AllureInProcessApi.UpdateTestResult(Action{TestResult})"/>.
    /// </summary>
    public AllureLifecycle UpdateTestCase(Action<TestResult> update)
    {
        AllureInProcessApi.UpdateTestResult(update);
        return this;
    }

    /// <summary>
    /// Please, switch to <see cref="AllureInProcessApi.UpdateStepResult(Action{StepResult})"/>.
    /// </summary>
    public AllureLifecycle UpdateStep(Action<StepResult> update)
    {
        AllureInProcessApi.UpdateStepResult(update);
        return this;
    }
}