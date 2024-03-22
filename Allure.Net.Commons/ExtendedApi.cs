using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Allure.Net.Commons.Functions;

#nullable enable

namespace Allure.Net.Commons;

/// <summary>
/// A facade that provides some advanced and/or low-level Runtime API to
/// enhance the report. You rarely need to use it directly. Please, always
/// prefer <see cref="AllureApi"/> when possible.
/// </summary>
public static class ExtendedApi
{
    internal static AllureLifecycle CurrentLifecycle
    {
        get => AllureApi.CurrentLifecycle;
    }

    #region Low-level fixtures API

    /// <summary>
    /// Starts a new setup fixture. Requires the container context to be
    /// active. Makes the fixture context active (if it wasn't). Deactivates
    /// the step context (if any).
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context is already active.
    /// </remarks>
    /// <param name="name">The name of the setup fixture.</param>
    public static void StartBeforeFixture(string name)
    {
        CurrentLifecycle.StartBeforeFixture(new() { name = name });
    }

    /// <summary>
    /// Starts a new teardown fixture. Requires the container context to be
    /// active. Makes the fixture context active (if it wasn't). Deactivates
    /// the step context (if any).
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context is already active.
    /// </remarks>
    /// <param name="name">The name of the teardown fixture.</param>
    public static void StartAfterFixture(string name)
    {
        CurrentLifecycle.StartAfterFixture(new() { name = name });
    }

    /// <summary>
    /// Stops the current fixture making it passed. Deactivates the step and
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context isn't active.
    /// </remarks>
    public static void PassFixture() => CurrentLifecycle.StopFixture(
        result =>
        {
            result.status = Status.passed;
        }
    );

    /// <summary>
    /// Stops the current fixture making it passed. Deactivates the step and
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context isn't active.
    /// </remarks>
    /// <param name="updateResults">
    /// The callback that is called before the fixture is stopped.
    /// </param>
    public static void PassFixture(Action<FixtureResult> updateResults)
    {
        CurrentLifecycle.UpdateFixture(updateResults);
        PassFixture();
    }

    /// <summary>
    /// Stops the current fixture making it failed. Deactivates the step and
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context isn't active.
    /// </remarks>
    public static void FailFixture() => CurrentLifecycle.StopFixture(
        result =>
        {
            result.status = Status.failed;
        }
    );

    /// <summary>
    /// Stops the current fixture making it failed. Deactivates the step and
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context isn't active.
    /// </remarks>
    /// <param name="updateResults">
    /// The callback that is called before the fixture is stopped.
    /// </param>
    public static void FailFixture(Action<FixtureResult> updateResults)
    {
        CurrentLifecycle.UpdateFixture(updateResults);
        FailFixture();
    }

    /// <summary>
    /// Stops the current fixture making it failed. Deactivates the step and
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context isn't active.
    /// </remarks>
    /// <param name="error">The error to report at the fixture level.</param>
    public static void FailFixture(Exception error) => CurrentLifecycle.StopFixture(
        result =>
        {
            result.status = Status.failed;
            result.statusDetails = ModelFunctions.ToStatusDetails(error);
        }
    );

    /// <summary>
    /// Stops the current fixture making it failed. Deactivates the step and
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context isn't active.
    /// </remarks>
    /// <param name="updateResults">
    /// The callback that is called before the fixture is stopped.
    /// </param>
    /// <param name="error">The error to report at the fixture level.</param>
    public static void FailFixture(
        Action<FixtureResult> updateResults,
        Exception error
    )
    {
        CurrentLifecycle.UpdateFixture(updateResults);
        FailFixture(error);
    }

    /// <summary>
    /// Stops the current fixture making it broken. Deactivates the step and
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context isn't active.
    /// </remarks>
    public static void BreakFixture() => CurrentLifecycle.StopFixture(
        result =>
        {
            result.status = Status.broken;
        }
    );

    /// <summary>
    /// Stops the current fixture making it broken. Deactivates the step and
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context isn't active.
    /// </remarks>
    /// <param name="updateResults">
    /// The callback that is called before the fixture is stopped.
    /// </param>
    public static void BreakFixture(Action<FixtureResult> updateResults)
    {
        CurrentLifecycle.UpdateFixture(updateResults);
        BreakFixture();
    }

    /// <summary>
    /// Stops the current fixture making it broken. Deactivates the step and
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context isn't active.
    /// </remarks>
    /// <param name="error">The error to report at the fixture level.</param>
    public static void BreakFixture(Exception error) => CurrentLifecycle.StopFixture(
        result =>
        {
            result.status = Status.broken;
            result.statusDetails = ModelFunctions.ToStatusDetails(error);
        }
    );

    /// <summary>
    /// Stops the current fixture making it broken. Deactivates the step and
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context isn't active.
    /// </remarks>
    /// <param name="updateResults">
    /// The callback that is called before the fixture is stopped.
    /// </param>
    /// <param name="error">The error to report at the fixture level.</param>
    public static void BreakFixture(
        Action<FixtureResult> updateResults,
        Exception error
    )
    {
        CurrentLifecycle.UpdateFixture(updateResults);
        BreakFixture(error);
    }

    /// <summary>
    /// Stops the current fixture making it skipped. Deactivates the step and
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context isn't active.
    /// </remarks>
    public static void SkipFixture() => CurrentLifecycle.StopFixture(
        result => result.status = Status.skipped
    );

    /// <summary>
    /// Stops the current fixture making it skipped. Deactivates the step and
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context isn't active.
    /// </remarks>
    /// <param name="updateResults">
    /// The callback that is called before the fixture is stopped.
    /// </param>
    public static void SkipFixture(Action<FixtureResult> updateResults) =>
        CurrentLifecycle.StopFixture(result =>
        {
            updateResults(result);
            result.status = Status.skipped;
        });

    /// <summary>
    ///   Stops the current fixture making it passed, failed, or broken
    ///   depending on the provided exception. Deactivates the current fixture
    ///   context.
    /// </summary>
    /// <remarks>
    ///   Requires the fixture context to be active.
    /// </remarks>
    /// <param name="error">
    ///   An exception instance. If it's null, the fixture is marked as passed.
    ///   Otherwise, the fixture is marked as failed or broken depending on the
    ///   exception's type and the configuration of the current lifecycle
    ///   instance.
    /// </param>
    public static void ResolveFixture(Exception? error) =>
        ResolveItem(CurrentLifecycle.StopFixture, error);

    #endregion

    #region Low-level steps API

    /// <summary>
    /// Starts a new step. Requires the fixture, the test or the step context
    /// to be active. Makes the step context active (if it wasn't).
    /// </summary>
    /// <param name="name">The name of the step.</param>
    public static void StartStep(string name)
    {
        CurrentLifecycle.StartStep(new() { name = name });
    }

    /// <summary>
    /// Starts a new step. Requires the fixture, the test or the step context
    /// to be active. Makes the step context active (if it wasn't).
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="updateResults">
    /// The callback that is executed right after the step is started.
    /// </param>
    public static void StartStep(string name, Action<StepResult> updateResults)
    {
        StartStep(name);
        CurrentLifecycle.UpdateStep(updateResults);
    }

    /// <summary>
    /// Stops the current step making it passed. Requires the step context to
    /// be active.
    /// </summary>
    public static void PassStep() => CurrentLifecycle.StopStep(
        result =>
        {
            result.status = Status.passed;
        }
    );

    /// <summary>
    /// Stops the current step making it passed. Requires the step context to
    /// be active.
    /// </summary>
    /// <param name="updateResults">
    /// The callback that is executed before the step is stopped.
    /// </param>
    public static void PassStep(Action<StepResult> updateResults)
    {
        CurrentLifecycle.UpdateStep(updateResults);
        PassStep();
    }

    /// <summary>
    /// Stops the current step making it failed. Requires the step context to
    /// be active.
    /// </summary>
    public static void FailStep() => CurrentLifecycle.StopStep(
        result =>
        {
            result.status = Status.failed;
        }
    );

    /// <summary>
    /// Stops the current step making it failed. Requires the step context to
    /// be active.
    /// </summary>
    /// <param name="updateResults">
    /// The callback that is executed before the step is stopped.
    /// </param>
    public static void FailStep(Action<StepResult> updateResults)
    {
        CurrentLifecycle.UpdateStep(updateResults);
        FailStep();
    }

    /// <summary>
    /// Stops the current step making it failed. Requires the step context to
    /// be active.
    /// </summary>
    /// <param name="error">The error to report at the step level.</param>
    public static void FailStep(Exception error) => CurrentLifecycle.StopStep(
        result =>
        {
            result.status = Status.failed;
            result.statusDetails = ModelFunctions.ToStatusDetails(error);
        }
    );

    /// <summary>
    /// Stops the current step making it failed. Requires the step context to
    /// be active.
    /// </summary>
    /// <param name="updateResults">
    /// The callback that is executed before the step is stopped.
    /// </param>
    /// <param name="error">The error to report at the step level.</param>
    public static void FailStep(
        Action<StepResult> updateResults,
        Exception error
    )
    {
        CurrentLifecycle.UpdateStep(updateResults);
        FailStep(error);
    }

    /// <summary>
    /// Stops the current step making it broken. Requires the step context to
    /// be active.
    /// </summary>
    public static void BreakStep() => CurrentLifecycle.StopStep(
        result =>
        {
            result.status = Status.broken;
        }
    );

    /// <summary>
    /// Stops the current step making it broken. Requires the step context to
    /// be active.
    /// </summary>
    /// <param name="updateResults">
    /// The callback that is executed before the step is stopped.
    /// </param>
    public static void BreakStep(Action<StepResult> updateResults)
    {
        CurrentLifecycle.UpdateStep(updateResults);
        BreakStep();
    }

    /// <summary>
    /// Stops the current step making it broken. Requires the step context to
    /// be active.
    /// </summary>
    /// <param name="error">The error to report at the step level.</param>
    public static void BreakStep(Exception error) => CurrentLifecycle.StopStep(
        result =>
        {
            result.status = Status.broken;
            result.statusDetails = ModelFunctions.ToStatusDetails(error);
        }
    );

    /// <summary>
    /// Stops the current step making it broken. Requires the step context to
    /// be active.
    /// </summary>
    /// <param name="updateResults">
    /// The callback that is executed before the step is stopped.
    /// </param>
    /// <param name="error">The error to report at the step level.</param>
    public static void BreakStep(
        Action<StepResult> updateResults,
        Exception error
    )
    {
        CurrentLifecycle.UpdateStep(updateResults);
        BreakStep(error);
    }

    /// <summary>
    /// Stops the current step making it skipped. Requires the step context to
    /// be active.
    /// </summary>
    public static void SkipStep() => CurrentLifecycle.StopStep(
        result => result.status = Status.skipped
    );

    /// <summary>
    /// Stops the current step making it skipped. Requires the step context to
    /// be active.
    /// </summary>
    /// <param name="updateResults">
    /// The callback that is executed before the step is stopped.
    /// </param>
    public static void SkipStep(Action<StepResult> updateResults) =>
        CurrentLifecycle.StopStep(result =>
        {
            updateResults(result);
            result.status = Status.skipped;
        });

    /// <summary>
    ///   Stops the current step making it passed, failed, or broken depending on
    ///   the provided exception.
    ///   Requires the step context to be active.
    /// </summary>
    /// <param name="error">
    ///   An exception instance. If it's null, the step is marked as passed.
    ///   Otherwise, the step is marked as failed or broken depending on the
    ///   exception's type and the configuration of the current lifecycle
    ///   instance.
    /// </param>
    public static void ResolveStep(Exception? error) =>
        ResolveItem(CurrentLifecycle.StopStep, error);

    #endregion

    #region Lambda fixtures

    /// <summary>
    /// Executes the action and reports the result as a new setup fixture.
    /// Requires the container context to be active.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context is already active.
    /// The current step context (if any) is deactivated.
    /// </remarks>
    /// <param name="name">The name of the setup fixture.</param>
    /// <param name="action">The code to run.</param>
    public static void Before(string name, Action action) =>
        Before(name, () =>
        {
            action();
            return null as object;
        });

    /// <summary>
    /// Executes the function and reports the result as a new setup fixture.
    /// Requires the container context to be active.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context is already active.
    /// The current step context (if any) is deactivated.
    /// </remarks>
    /// <param name="name">The name of the setup fixture.</param>
    /// <param name="function">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static T Before<T>(string name, Func<T> function) =>
        ExecuteFixture(name, StartBeforeFixture, function);

    /// <summary>
    /// Executes the asynchronous action and reports the result as a new setup
    /// fixture.
    /// Requires the container context to be active.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context is already active.
    /// The current step context (if any) is deactivated.
    /// </remarks>
    /// <param name="name">The name of the setup fixture.</param>
    /// <param name="action">The asynchronous code to run.</param>
    public static async Task Before(string name, Func<Task> action) =>
        await ExecuteFixtureAsync(name, StartBeforeFixture, async () =>
        {
            await action();
            return Task.FromResult<object?>(null);
        });

    /// <summary>
    /// Executes the asynchronous function and reports the result as a new
    /// setup fixture.
    /// Requires the container context to be active.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context is already active.
    /// The current step context (if any) is deactivated.
    /// </remarks>
    /// <param name="name">The name of the setup fixture.</param>
    /// <param name="function">The asynchronous function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static async Task<T> Before<T>(string name, Func<Task<T>> function) =>
        await ExecuteFixtureAsync(name, StartBeforeFixture, function);

    /// <summary>
    /// Executes the action and reports the result as a new teardown fixture.
    /// Requires the container context to be active.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context is already active.
    /// The current step context (if any) is deactivated.
    /// </remarks>
    /// <param name="name">The name of the teardown fixture.</param>
    /// <param name="action">The code to run.</param>
    public static void After(string name, Action action) =>
        After(name, () =>
        {
            action();
            return null as object;
        });

    /// <summary>
    /// Executes the function and reports the result as a new teardown fixture.
    /// Requires the container context to be active.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context is already active.
    /// The current step context (if any) is deactivated.
    /// </remarks>
    /// <param name="name">The name of the teardown fixture.</param>
    /// <param name="function">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static T After<T>(string name, Func<T> function) =>
        ExecuteFixture(name, StartAfterFixture, function);

    /// <summary>
    /// Executes the asynchronous action and reports the result as a new
    /// teardown fixture.
    /// Requires the container context to be active.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context is already active.
    /// The current step context (if any) is deactivated.
    /// </remarks>
    /// <param name="name">The name of the teardown fixture.</param>
    /// <param name="action">The asynchronous code to run.</param>
    public static async Task After(string name, Func<Task> action) =>
        await ExecuteFixtureAsync(name, StartAfterFixture, async () =>
        {
            await action();
            return Task.FromResult<object?>(null);
        });

    /// <summary>
    /// Executes the asynchronous function and reports the result as a new
    /// teardown fixture.
    /// Requires the container context to be active.
    /// </summary>
    /// <remarks>
    /// Can't be called if the fixture context is already active.
    /// The current step context (if any) is deactivated.
    /// </remarks>
    /// <param name="name">The name of the teardown fixture.</param>
    /// <param name="function">The asynchronous function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static async Task<T> After<T>(string name, Func<Task<T>> function) =>
        await ExecuteFixtureAsync(name, StartAfterFixture, function);

    #endregion

    static IEnumerable<string> FailExceptions
    {
        get => CurrentLifecycle.AllureConfiguration.FailExceptions
            ?? Enumerable.Empty<string>();
    }

    static T ExecuteFixture<T>(
        string name,
        Action<string> start,
        Func<T> action
    ) =>
        AllureApi.ExecuteAction(
            name,
            start,
            action,
            resolve: ResolveFixture
        );

    static async Task<T> ExecuteFixtureAsync<T>(
        string name,
        Action<string> startFixture,
        Func<Task<T>> action
    ) =>
        await AllureApi.ExecuteActionAsync(
            () => startFixture(name),
            action,
            resolve: ResolveFixture
        );

    static void ResolveItem(
        Func<Action<ExecutableItem>, AllureLifecycle> stop,
        Exception? error
    )
    {
        var (status, statusDetails) = ResolveDetailedStatus(error);
        stop(item =>
        {
            item.status = status;
            item.statusDetails = statusDetails;
        });
    }

    static (Status, StatusDetails?) ResolveDetailedStatus(
        Exception? error
    ) =>
        error is null
            ? (Status.passed, null)
            : (
                ModelFunctions.ResolveErrorStatus(FailExceptions, error),
                ModelFunctions.ToStatusDetails(error)
            );
}
