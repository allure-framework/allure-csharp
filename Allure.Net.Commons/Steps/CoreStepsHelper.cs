using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Allure.Net.Commons.Storage;

#nullable enable

namespace Allure.Net.Commons.Steps;

public class CoreStepsHelper
{
    static AllureLifecycle? lifecycleInstance;

    internal static AllureLifecycle CurrentLifecycle
    {
        get => lifecycleInstance ?? AllureLifecycle.Instance;
        set => lifecycleInstance = value;
    }

    public static IStepLogger? StepLogger { get; set; }

    #region Fixtures

    public static void StartBeforeFixture(string name)
    {
        CurrentLifecycle.StartBeforeFixture(new() { name = name });
        StepLogger?.BeforeStarted?.Log(name);
    }

    public static void StartAfterFixture(string name)
    {
        CurrentLifecycle.StartAfterFixture(new() { name = name });
        StepLogger?.AfterStarted?.Log(name);
    }

    public static void PassFixture() => CurrentLifecycle.StopFixture(
        result =>
        {
            result.status = Status.passed;
            StepLogger?.StepPassed?.Log(result.name);
        }
    );

    public static void PassFixture(Action<FixtureResult> updateResults)
    {
        CurrentLifecycle.UpdateFixture(updateResults);
        PassFixture();
    }

    public static void FailFixture() => CurrentLifecycle.StopFixture(
        result =>
        {
            result.status = Status.failed;
            StepLogger?.StepFailed?.Log(result.name);
        }
    );

    public static void FailFixture(Action<FixtureResult> updateResults)
    {
        CurrentLifecycle.UpdateFixture(updateResults);
        FailFixture();
    }

    public static void BrokeFixture() => CurrentLifecycle.StopFixture(
        result =>
        {
            result.status = Status.broken;
            StepLogger?.StepBroken?.Log(result.name);
        }
    );

    public static void BrokeFixture(Action<FixtureResult> updateResults)
    {
        CurrentLifecycle.UpdateFixture(updateResults);
        BrokeFixture();
    }

    public static void StopFixture(Action<FixtureResult> updateResults) =>
        CurrentLifecycle.StopFixture(updateResults);

    public static void StopFixture() =>
        CurrentLifecycle.StopFixture();

    #endregion

    #region Steps

    public static void StartStep(string name)
    {
        CurrentLifecycle.StartStep(new() { name = name });
        StepLogger?.StepStarted?.Log(name);
    }

    public static void StartStep(string name, Action<StepResult> updateResults)
    {
        StartStep(name);
        CurrentLifecycle.UpdateStep(updateResults);
    }

    public static void PassStep() => CurrentLifecycle.StopStep(
        result =>
        {
            result.status = Status.passed;
            StepLogger?.StepPassed?.Log(result.name);
        }
    );

    public static void PassStep(Action<StepResult> updateResults)
    {
        CurrentLifecycle.UpdateStep(updateResults);
        PassStep();
    }

    public static void FailStep() => CurrentLifecycle.StopStep(
        result =>
        {
            result.status = Status.failed;
            StepLogger?.StepFailed?.Log(result.name);
        }
    );

    public static void FailStep(Action<StepResult> updateResults)
    {
        CurrentLifecycle.UpdateStep(updateResults);
        FailStep();
    }

    public static void BrokeStep() => CurrentLifecycle.StopStep(
        result =>
        {
            result.status = Status.broken;
            StepLogger?.StepBroken?.Log(result.name);
        }
    );

    public static void BrokeStep(Action<StepResult> updateResults)
    {
        CurrentLifecycle.UpdateStep(updateResults);
        BrokeStep();
    }

    #endregion

    #region Misc

    public static void UpdateTestResult(Action<TestResult> update) =>
        CurrentLifecycle.UpdateTestCase(update);

    #endregion

    public static async Task<T> Step<T>(string name, Func<Task<T>> action) =>
        await ExecuteStep(name, action);

    public static T Step<T>(string name, Func<T> action) =>
        ExecuteStep(name, action);

    public static void Step(string name, Action action)
    {
        ExecuteStep(name, () =>
        {
            action();
            return null as object;
        });
    }

    public static async Task Step(string name, Func<Task> action) =>
        await ExecuteStep(name, async () =>
        {
            await action();
            return Task.FromResult<object?>(null);
        });

    public static void Step(string name) =>
        Step(name, () => { });

    public static async Task<T> Before<T>(string name, Func<Task<T>> action) =>
        await ExecuteFixture(name, StartBeforeFixture, action);

    public static T Before<T>(string name, Func<T> action) =>
        ExecuteFixture(name, StartBeforeFixture, action);

    public static void Before(string name, Action action) =>
        Before(name, () =>
        {
            action();
            return null as object;
        });

    public static async Task Before(string name, Func<Task> action) =>
        await ExecuteFixture(name, StartBeforeFixture, async () =>
        {
            await action();
            return Task.FromResult<object?>(null);
        });

    public static async Task<T> After<T>(string name, Func<Task<T>> action) =>
        await ExecuteFixture(name, StartAfterFixture, action);

    public static T After<T>(string name, Func<T> action) =>
        ExecuteFixture(name, StartAfterFixture, action);

    public static void After(string name, Action action) =>
        After(name, () =>
        {
            action();
            return null as object;
        });

    public static async Task After(string name, Func<Task> action) =>
        await ExecuteFixture(name, StartAfterFixture, async () =>
        {
            await action();
            return Task.FromResult<object?>(null);
        });

    static T ExecuteStep<T>(string name, Func<T> action) =>
        ExecuteAction(
            name,
            StartStep,
            action,
            PassStep,
            FailStep
        );

    static async Task<T> ExecuteStep<T>(
        string name,
        Func<Task<T>> action
    ) =>
        await ExecuteAction(
            () => StartStep(name),
            action,
            PassStep,
            FailStep
        );

    static T ExecuteFixture<T>(
        string name,
        Action<string> start,
        Func<T> action
    ) =>
        ExecuteAction(
            name,
            start,
            action,
            PassFixture,
            FailFixture
        );

    static async Task<T> ExecuteFixture<T>(
        string name,
        Action<string> startFixture,
        Func<Task<T>> action
    ) =>
        await ExecuteAction(
            () => startFixture(name),
            action,
            PassFixture,
            FailFixture
        );

    private static async Task<T> ExecuteAction<T>(
        Action start,
        Func<Task<T>> action,
        Action pass,
        Action fail
    )
    {
        T result;
        start();
        try
        {
            result = await action();
        }
        catch (Exception)
        {
            fail();
            throw;
        }

        pass();
        return result;
    }

    private static T ExecuteAction<T>(
        string name,
        Action<string> start,
        Func<T> action,
        Action pass,
        Action fail
    )
    {
        T result;
        start(name);
        try
        {
            result = action();
        }
        catch (Exception e)
        {
            fail();
            throw new StepFailedException(name, e);
        }

        pass();
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
        CurrentLifecycle.UpdateStep(uuid, result =>
        {
            result.status = Status.passed;
            updateResults?.Invoke(result);
            StepLogger?.StepPassed?.Log(result.name);
        });
        CurrentLifecycle.StopStep(uuid);
    }

    [Obsolete(AllureLifecycle.EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void FailStep(
        string uuid,
        Action<StepResult>? updateResults = null
    )
    {
        CurrentLifecycle.UpdateStep(uuid, result =>
        {
            result.status = Status.failed;
            updateResults?.Invoke(result);
            StepLogger?.StepFailed?.Log(result.name);
        });
        CurrentLifecycle.StopStep(uuid);
    }

    [Obsolete(AllureLifecycle.EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void BrokeStep(
        string uuid,
        Action<StepResult>? updateResults = null
    )
    {
        CurrentLifecycle.UpdateStep(uuid, result =>
        {
            result.status = Status.broken;
            updateResults?.Invoke(result);
            StepLogger?.StepBroken?.Log(result.name);
        });
        CurrentLifecycle.StopStep(uuid);
    }

    #endregion
}