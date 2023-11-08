using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Allure.Net.Commons.Storage;

#nullable enable

namespace Allure.Net.Commons.Steps;

[Obsolete("Members of this class are now a part of the end user API represented by the AllureApi facade. " +
    "Please, use the Allure.Net.Commons.AllureApi class instead.")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class CoreStepsHelper
{
    [Obsolete("Please, use AllureApi.StepLogger instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IStepLogger? StepLogger
    {
        get => AllureApi.StepLogger;
        set => AllureApi.StepLogger = value;
    }

    [Obsolete("Please, use AllureApi.StartBeforeFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StartBeforeFixture(string name) =>
        AllureApi.StartBeforeFixture(name);

    [Obsolete("Please, use AllureApi.StartAfterFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StartAfterFixture(string name) =>
        AllureApi.StartAfterFixture(name);

    [Obsolete("Please, use AllureApi.PassFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void PassFixture() =>
        AllureApi.PassFixture();

    [Obsolete("Please, use AllureApi.PassFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void PassFixture(Action<FixtureResult> updateResults) =>
        AllureApi.PassFixture(updateResults);

    [Obsolete("Please, use AllureApi.FailFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void FailFixture() =>
        AllureApi.FailFixture();

    [Obsolete("Please, use AllureApi.FailFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void FailFixture(Action<FixtureResult> updateResults) =>
        AllureApi.FailFixture(updateResults);

    [Obsolete("Please, use AllureApi.BreakFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void BrokeFixture() => 
        AllureApi.BreakFixture();

    [Obsolete("Please, use AllureApi.BreakFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void BrokeFixture(Action<FixtureResult> updateResults) =>
        AllureApi.BreakFixture(updateResults);

    [Obsolete("Please, use AllureLifecycle.StopFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StopFixture(Action<FixtureResult> updateResults) =>
        AllureLifecycle.Instance.StopFixture(updateResults);

    [Obsolete("Please, use AllureLifecycle.StopFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StopFixture() =>
        AllureLifecycle.Instance.StopFixture();

    [Obsolete("Please, use AllureApi.StartStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StartStep(string name) =>
        AllureApi.StartStep(name);

    [Obsolete("Please, use AllureApi.StartStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StartStep(string name, Action<StepResult> updateResults) =>
        AllureApi.StartStep(name, updateResults);

    [Obsolete("Please, use AllureApi.PassStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void PassStep() =>
        AllureApi.PassStep();

    [Obsolete("Please, use AllureApi.PassStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void PassStep(Action<StepResult> updateResults) =>
        AllureApi.PassStep(updateResults);

    [Obsolete("Please, use AllureApi.FailStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void FailStep() =>
        AllureApi.FailStep();

    [Obsolete("Please, use AllureApi.FailStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void FailStep(Action<StepResult> updateResults) =>
        AllureApi.FailStep(updateResults);

    [Obsolete("Please, use AllureApi.BreakStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void BrokeStep() =>
        AllureApi.BreakStep();

    [Obsolete("Please, use AllureApi.BreakStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void BrokeStep(Action<StepResult> updateResults) =>
        AllureApi.BreakStep(updateResults);

    [Obsolete("Please, use AllureLifecycle.UpdateTestCase instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void UpdateTestResult(Action<TestResult> update) =>
        AllureLifecycle.Instance.UpdateTestCase(update);

    [Obsolete("Please, use AllureApi.Step instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Step(string name) =>
        AllureApi.Step(name);

    [Obsolete("Please, use AllureApi.Step instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Step(string name, Action action) =>
        AllureApi.Step(name, action);

    [Obsolete("Please, use AllureApi.Step instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T Step<T>(string name, Func<T> action) =>
        AllureApi.Step(name, action);

    [Obsolete("Please, use AllureApi.Step instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task Step(string name, Func<Task> action) =>
        await AllureApi.Step(name, action);

    [Obsolete("Please, use AllureApi.Step instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task<T> Step<T>(string name, Func<Task<T>> action) =>
        await AllureApi.Step(name, action);

    [Obsolete("Please, use AllureApi.Before instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Before(string name, Action action) =>
        AllureApi.Before(name, action);

    [Obsolete("Please, use AllureApi.Before instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T Before<T>(string name, Func<T> action) =>
        AllureApi.Before(name, action);

    [Obsolete("Please, use AllureApi.Before instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task Before(string name, Func<Task> action) =>
        await AllureApi.Before(name, action);

    [Obsolete("Please, use AllureApi.Before instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task<T> Before<T>(string name, Func<Task<T>> action) =>
        await AllureApi.Before(name, action);

    [Obsolete("Please, use AllureApi.After instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void After(string name, Action action) =>
        AllureApi.After(name, action);

    [Obsolete("Please, use AllureApi.After instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T After<T>(string name, Func<T> action) =>
        AllureApi.After(name, action);

    [Obsolete("Please, use AllureApi.After instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task After(string name, Func<Task> action) =>
        await AllureApi.After(name, action);

    [Obsolete("Please, use AllureApi.After instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task<T> After<T>(string name, Func<Task<T>> action) =>
        await AllureApi.After(name, action);

    [Obsolete(AllureLifecycle.API_RUDIMENT_OBSOLETE_MSG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ITestResultAccessor? TestResultAccessor { get; set; }

    [Obsolete(
        "This method is a rudimentary part of the API and will be removed " +
            "in the future. Use the StopFixture method instead."
    )]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StopFixtureSuppressTestCase(
        Action<FixtureResult>? updateResults = null
    )
    {
        if (updateResults == null)
        {
            StopFixture();
        }
        else
        {
            StopFixture(updateResults);
        }
    }

    [Obsolete(AllureLifecycle.EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void PassStep(
        string uuid,
        Action<StepResult>? updateResults = null
    )
    {
        AllureLifecycle.Instance.UpdateStep(uuid, result =>
        {
            result.status = Status.passed;
            updateResults?.Invoke(result);
            AllureApi.StepLogger?.StepPassed?.Log(result.name);
        });
        AllureLifecycle.Instance.StopStep(uuid);
    }

    [Obsolete(AllureLifecycle.EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void FailStep(
        string uuid,
        Action<StepResult>? updateResults = null
    )
    {
        AllureLifecycle.Instance.UpdateStep(uuid, result =>
        {
            result.status = Status.failed;
            updateResults?.Invoke(result);
            AllureApi.StepLogger?.StepFailed?.Log(result.name);
        });
        AllureLifecycle.Instance.StopStep(uuid);
    }

    [Obsolete(AllureLifecycle.EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void BrokeStep(
        string uuid,
        Action<StepResult>? updateResults = null
    )
    {
        AllureLifecycle.Instance.UpdateStep(uuid, result =>
        {
            result.status = Status.broken;
            updateResults?.Invoke(result);
            AllureApi.StepLogger?.StepBroken?.Log(result.name);
        });
        AllureLifecycle.Instance.StopStep(uuid);
    }
}