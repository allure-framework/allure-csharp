using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Allure.Net.Commons.Storage;

#nullable enable

namespace Allure.Net.Commons.Steps;

[Obsolete("Members of this class are now a part of the end user API represented by the Allure facade. " +
    "Please, use the Allure.Net.Commons.Allure class instead.")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class CoreStepsHelper
{
    [Obsolete("Please, use Allure.StepLogger instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IStepLogger? StepLogger
    {
        get => Allure.StepLogger;
        set => Allure.StepLogger = value;
    }

    [Obsolete("Please, use Allure.StartBeforeFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StartBeforeFixture(string name) =>
        Allure.StartBeforeFixture(name);

    [Obsolete("Please, use Allure.StartAfterFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StartAfterFixture(string name) =>
        Allure.StartAfterFixture(name);

    [Obsolete("Please, use Allure.PassFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void PassFixture() =>
        Allure.PassFixture();

    [Obsolete("Please, use Allure.PassFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void PassFixture(Action<FixtureResult> updateResults) =>
        Allure.PassFixture(updateResults);

    [Obsolete("Please, use Allure.FailFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void FailFixture() =>
        Allure.FailFixture();

    [Obsolete("Please, use Allure.FailFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void FailFixture(Action<FixtureResult> updateResults) =>
        Allure.FailFixture(updateResults);

    [Obsolete("Please, use Allure.BreakFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void BrokeFixture() => 
        Allure.BreakFixture();

    [Obsolete("Please, use Allure.BreakFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void BrokeFixture(Action<FixtureResult> updateResults) =>
        Allure.BreakFixture(updateResults);

    [Obsolete("Please, use AllureLifecycle.StopFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StopFixture(Action<FixtureResult> updateResults) =>
        AllureLifecycle.Instance.StopFixture(updateResults);

    [Obsolete("Please, use AllureLifecycle.StopFixture instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StopFixture() =>
        AllureLifecycle.Instance.StopFixture();

    [Obsolete("Please, use Allure.StartStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StartStep(string name) =>
        Allure.StartStep(name);

    [Obsolete("Please, use Allure.StartStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StartStep(string name, Action<StepResult> updateResults) =>
        Allure.StartStep(name, updateResults);

    [Obsolete("Please, use Allure.PassStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void PassStep() =>
        Allure.PassStep();

    [Obsolete("Please, use Allure.PassStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void PassStep(Action<StepResult> updateResults) =>
        Allure.PassStep(updateResults);

    [Obsolete("Please, use Allure.FailStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void FailStep() =>
        Allure.FailStep();

    [Obsolete("Please, use Allure.FailStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void FailStep(Action<StepResult> updateResults) =>
        Allure.FailStep(updateResults);

    [Obsolete("Please, use Allure.BreakStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void BrokeStep() =>
        Allure.BreakStep();

    [Obsolete("Please, use Allure.BreakStep instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void BrokeStep(Action<StepResult> updateResults) =>
        Allure.BreakStep(updateResults);

    [Obsolete("Please, use AllureLifecycle.UpdateTestCase instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void UpdateTestResult(Action<TestResult> update) =>
        AllureLifecycle.Instance.UpdateTestCase(update);

    [Obsolete("Please, use Allure.Step instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Step(string name) =>
        Allure.Step(name);

    [Obsolete("Please, use Allure.Step instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Step(string name, Action action) =>
        Allure.Step(name, action);

    [Obsolete("Please, use Allure.Step instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T Step<T>(string name, Func<T> action) =>
        Allure.Step(name, action);

    [Obsolete("Please, use Allure.Step instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task Step(string name, Func<Task> action) =>
        await Allure.Step(name, action);

    [Obsolete("Please, use Allure.Step instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task<T> Step<T>(string name, Func<Task<T>> action) =>
        await Allure.Step(name, action);

    [Obsolete("Please, use Allure.Before instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Before(string name, Action action) =>
        Allure.Before(name, action);

    [Obsolete("Please, use Allure.Before instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T Before<T>(string name, Func<T> action) =>
        Allure.Before(name, action);

    [Obsolete("Please, use Allure.Before instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task Before(string name, Func<Task> action) =>
        await Allure.Before(name, action);

    [Obsolete("Please, use Allure.Before instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task<T> Before<T>(string name, Func<Task<T>> action) =>
        await Allure.Before(name, action);

    [Obsolete("Please, use Allure.After instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void After(string name, Action action) =>
        Allure.After(name, action);

    [Obsolete("Please, use Allure.After instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T After<T>(string name, Func<T> action) =>
        Allure.After(name, action);

    [Obsolete("Please, use Allure.After instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task After(string name, Func<Task> action) =>
        await Allure.After(name, action);

    [Obsolete("Please, use Allure.After instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task<T> After<T>(string name, Func<Task<T>> action) =>
        await Allure.After(name, action);

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
            Allure.StepLogger?.StepPassed?.Log(result.name);
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
            Allure.StepLogger?.StepFailed?.Log(result.name);
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
            Allure.StepLogger?.StepBroken?.Log(result.name);
        });
        AllureLifecycle.Instance.StopStep(uuid);
    }
}