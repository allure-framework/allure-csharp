using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Allure.Net.Commons.Storage;

#nullable enable

namespace Allure.Net.Commons.Steps;

[Obsolete("Members of this class are now a part of the Runtime API represented by the AllureApi and ExtendedApi facades.")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class CoreStepsHelper
{
    [Obsolete("Step logging is obsolete. It doesn't do anything now and will be removed in a future release.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IStepLogger? StepLogger { get; set; }

    [Obsolete("Please, use AllureApi.StartBeforeFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StartBeforeFixture(string name) =>
        ExtendedApi.StartBeforeFixture(name);

    [Obsolete("Please, use AllureApi.StartAfterFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StartAfterFixture(string name) =>
        ExtendedApi.StartAfterFixture(name);

    [Obsolete("Please, use AllureApi.PassFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void PassFixture() =>
        ExtendedApi.PassFixture();

    [Obsolete("Please, use AllureApi.PassFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void PassFixture(Action<FixtureResult> updateResults) =>
        ExtendedApi.PassFixture(updateResults);

    [Obsolete("Please, use AllureApi.FailFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void FailFixture() =>
        ExtendedApi.FailFixture();

    [Obsolete("Please, use AllureApi.FailFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void FailFixture(Action<FixtureResult> updateResults) =>
        ExtendedApi.FailFixture(updateResults);

    [Obsolete("Please, use AllureApi.BreakFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void BrokeFixture() =>
        ExtendedApi.BreakFixture();

    [Obsolete("Please, use AllureApi.BreakFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void BrokeFixture(Action<FixtureResult> updateResults) =>
        ExtendedApi.BreakFixture(updateResults);

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
        ExtendedApi.StartStep(name);

    [Obsolete("Please, use AllureApi.StartStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StartStep(string name, Action<StepResult> updateResults) =>
        ExtendedApi.StartStep(name, updateResults);

    [Obsolete("Please, use AllureApi.PassStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void PassStep() =>
        ExtendedApi.PassStep();

    [Obsolete("Please, use AllureApi.PassStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void PassStep(Action<StepResult> updateResults) =>
        ExtendedApi.PassStep(updateResults);

    [Obsolete("Please, use AllureApi.FailStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void FailStep() =>
        ExtendedApi.FailStep();

    [Obsolete("Please, use AllureApi.FailStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void FailStep(Action<StepResult> updateResults) =>
        ExtendedApi.FailStep(updateResults);

    [Obsolete("Please, use AllureApi.BreakStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void BrokeStep() =>
        ExtendedApi.BreakStep();

    [Obsolete("Please, use AllureApi.BreakStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void BrokeStep(Action<StepResult> updateResults) =>
        ExtendedApi.BreakStep(updateResults);

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
        ExtendedApi.Before(name, action);

    [Obsolete("Please, use AllureApi.Before instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T Before<T>(string name, Func<T> action) =>
        ExtendedApi.Before(name, action);

    [Obsolete("Please, use AllureApi.Before instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task Before(string name, Func<Task> action) =>
        await ExtendedApi.Before(name, action);

    [Obsolete("Please, use AllureApi.Before instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task<T> Before<T>(string name, Func<Task<T>> action) =>
        await ExtendedApi.Before(name, action);

    [Obsolete("Please, use AllureApi.After instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void After(string name, Action action) =>
        ExtendedApi.After(name, action);

    [Obsolete("Please, use AllureApi.After instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T After<T>(string name, Func<T> action) =>
        ExtendedApi.After(name, action);

    [Obsolete("Please, use AllureApi.After instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task After(string name, Func<Task> action) =>
        await ExtendedApi.After(name, action);

    [Obsolete("Please, use AllureApi.After instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task<T> After<T>(string name, Func<Task<T>> action) =>
        await ExtendedApi.After(name, action);

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
        });
        AllureLifecycle.Instance.StopStep(uuid);
    }
}