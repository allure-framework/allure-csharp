using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Allure.Net.Commons.Storage;

#nullable enable

namespace Allure.Net.Commons.Steps;

public class CoreStepsHelper
{
    public static IStepLogger? StepLogger { get; set; }

    #region Fixtures

    public static void StartBeforeFixture(string name)
    {
        AllureLifecycle.Instance.StartBeforeFixture(new() { name = name });
        StepLogger?.BeforeStarted?.Log(name);
    }

    public static void StartAfterFixture(string name)
    {
        AllureLifecycle.Instance.StartAfterFixture(new() { name = name });
        StepLogger?.AfterStarted?.Log(name);
    }

    public static void StopFixture(Action<FixtureResult> updateResults) =>
        AllureLifecycle.Instance.StopFixture(updateResults);

    public static void StopFixture() =>
        AllureLifecycle.Instance.StopFixture();

    #endregion

    #region Steps

    public static void StartStep(string name)
    {
        AllureLifecycle.Instance.StartStep(new() { name = name });
        StepLogger?.StepStarted?.Log(name);
    }

    public static void StartStep(string name, Action<StepResult> updateResults)
    {
        StartStep(name);
        AllureLifecycle.Instance.UpdateStep(updateResults);
    }

    public static void PassStep() => AllureLifecycle.Instance.StopStep(
        result =>
        {
            result.status = Status.passed;
            StepLogger?.StepPassed?.Log(result.name);
        }
    );

    public static void PassStep(Action<StepResult> updateResults)
    {
        AllureLifecycle.Instance.UpdateStep(updateResults);
        PassStep();
    }

    public static void FailStep() => AllureLifecycle.Instance.StopStep(
        result =>
        {
            result.status = Status.failed;
            StepLogger?.StepFailed?.Log(result.name);
        }
    );

    public static void FailStep(Action<StepResult> updateResults)
    {
        AllureLifecycle.Instance.UpdateStep(updateResults);
        FailStep();
    }

    public static void BrokeStep() => AllureLifecycle.Instance.StopStep(
        result =>
        {
            result.status = Status.broken;
            StepLogger?.StepBroken?.Log(result.name);
        }
    );

    public static void BrokeStep(Action<StepResult> updateResults)
    {
        AllureLifecycle.Instance.UpdateStep(updateResults);
        BrokeStep();
    }

    #endregion

    #region Misc

    public static void UpdateTestResult(Action<TestResult> update) =>
        AllureLifecycle.Instance.UpdateTestCase(update);

    #endregion

    public static Task<T> Step<T>(string name, Func<Task<T>> action)
    {
        StartStep(name);
        return Execute(action);
    }

    public static T Step<T>(string name, Func<T> action)
    {
        StartStep(name);
        return Execute(name, action);
    }

    public static void Step(string name, Action action)
    {
        Step(name, (Func<object?>)(() =>
        {
            action();
            return null;
        }));
    }

    public static Task Step(string name, Func<Task> action)
    {
        return Step(name, async () =>
        {
            await action();
            return Task.FromResult<object?>(null);
        });
    }

    public static void Step(string name)
    {
        Step(name, () => { });
    }

    public static Task<T> Before<T>(string name, Func<Task<T>> action)
    {
        StartBeforeFixture(name);
        return Execute(action);
    }

    public static T Before<T>(string name, Func<T> action)
    {
        StartBeforeFixture(name);
        return Execute(name, action);
    }

    public static void Before(string name, Action action)
    {
        Before(name, (Func<object?>)(() =>
        {
            action();
            return null;
        }));
    }

    public static Task Before(string name, Func<Task> action)
    {
        return Before(name, async () =>
        {
            await action();
            return Task.FromResult<object?>(null);
        });
    }

    public static Task<T> After<T>(string name, Func<Task<T>> action)
    {
        StartAfterFixture(name);
        return Execute(action);
    }

    public static T After<T>(string name, Func<T> action)
    {
        StartAfterFixture(name);
        return Execute(name, action);
    }

    public static void After(string name, Action action)
    {
        After(name, (Func<object?>)(() =>
        {
            action();
            return null;
        }));
    }

    public static Task After(string name, Func<Task> action)
    {
        return After(name, async () =>
        {
            await action();
            return Task.FromResult<object?>(null);
        });
    }

    private static async Task<T> Execute<T>(Func<Task<T>> action)
    {
        T result;
        try
        {
            result = await action();
        }
        catch (Exception)
        {
            FailStep();
            throw;
        }

        PassStep();
        return result;
    }

    private static T Execute<T>(string name, Func<T> action)
    {
        T result;
        try
        {
            result = action();
        }
        catch (Exception e)
        {
            FailStep();
            throw new StepFailedException(name, e);
        }

        PassStep();
        return result;
    }

    #region Obsoleted

    [Obsolete(AllureLifecycle.API_RUDIMENT_OBSOLETE_MSG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ITestResultAccessor? TestResultAccessor { get; set; }

    [Obsolete(
        "This method is a rudimentary part of the API and will be removed " +
            "in the future. Use the StopFixture method instead."
    )]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StopFixtureSuppressTestCase() => StopFixture();

    [Obsolete(
        "This method is a rudimentary part of the API and will be removed " +
            "in the future. Use the StopFixture method instead."
    )]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void StopFixtureSuppressTestCase(
        Action<FixtureResult> updateResults
    ) => StopFixture(updateResults);

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
            StepLogger?.StepPassed?.Log(result.name);
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
            StepLogger?.StepFailed?.Log(result.name);
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
            StepLogger?.StepBroken?.Log(result.name);
        });
        AllureLifecycle.Instance.StopStep(uuid);
    }

    #endregion
}