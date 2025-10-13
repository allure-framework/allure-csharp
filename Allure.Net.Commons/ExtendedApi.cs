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
    static AllureLifecycle CurrentLifecycle
    {
        get => AllureApi.CurrentLifecycle;
    }

    static AllureContext Context => CurrentLifecycle.Context;

    internal static bool HasContainer => Context.HasContainer;
    internal static bool HasTest => Context.HasTest;
    internal static bool HasFixture => Context.HasFixture;
    internal static bool HasStep => Context.HasStep;
    internal static bool HasTestOrFixture => HasTest || HasFixture;

    #region Low-level fixtures API

    /// <summary>
    /// Starts a new setup fixture. Removes all unfinished steps.
    /// </summary>
    /// <remarks>
    /// If Allure is not running, does nothing.
    /// </remarks>
    /// <param name="name">The name of the setup fixture.</param>
    public static void StartBeforeFixture(string name)
    {
        if (HasContainer)
        {
            StartBeforeFixtureInternal(name);
        }
    }

    /// <summary>
    /// Starts a new teardown fixture. Removes all unfinished steps.
    /// </summary>
    /// <remarks>
    /// If Allure is not running, does nothing.
    /// </remarks>
    /// <param name="name">The name of the teardown fixture.</param>
    public static void StartAfterFixture(string name)
    {
        if (HasContainer)
        {
            StartAfterFixtureInternal(name);
        }
    }

    /// <summary>
    /// Makes the current fixture passed and stops it. Removes all unfinished steps.
    /// </summary>
    /// <remarks>
    /// If no fixture is running, does nothing.
    /// </remarks>
    public static void PassFixture()
    {
        if (HasFixture)
        {
            PassFixtureInternal();
        }
    }

    /// <summary>
    /// Makes the current fixture passed and stops it. Removes all unfinished steps.
    /// </summary>
    /// <remarks>
    /// If no fixture is running, does nothing.
    /// </remarks>
    /// <param name="updateResults">
    /// The callback that is called before the fixture is stopped.
    /// </param>
    public static void PassFixture(Action<FixtureResult> updateResults)
    {
        if (HasFixture)
        {
            CurrentLifecycle.UpdateFixture(updateResults);
            PassFixtureInternal();
        }
    }

    /// <summary>
    /// Makes the current fixture failed and stops it. Removes all unfinished steps.
    /// </summary>
    /// <remarks>
    /// If no fixture is running, does nothing.
    /// </remarks>
    public static void FailFixture()
    {
        if (HasFixture)
        {
            FailFixtureInternal();
        }
    }

    /// <summary>
    /// Makes the current fixture failed and stops it. Removes all unfinished steps.
    /// </summary>
    /// <remarks>
    /// If no fixture is running, does nothing.
    /// </remarks>
    /// <param name="updateResults">
    /// The callback that is called before the fixture is stopped.
    /// </param>
    public static void FailFixture(Action<FixtureResult> updateResults)
    {
        if (HasFixture)
        {
            CurrentLifecycle.UpdateFixture(updateResults);
            FailFixtureInternal();
        }
    }

    /// <summary>
    /// Makes the current fixture failed and stops it. Removes all unfinished steps.
    /// </summary>
    /// <remarks>
    /// If no fixture is running, does nothing.
    /// </remarks>
    /// <param name="error">The error to report at the fixture level.</param>
    public static void FailFixture(Exception error)
    {
        if (HasFixture)
        {
            FailFixtureInternal(error);
        }
    }

    /// <summary>
    /// Makes the current fixture failed and stops it. Removes all unfinished steps.
    /// </summary>
    /// <remarks>
    /// If no fixture is running, does nothing.
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
        if (HasFixture)
        {
            CurrentLifecycle.UpdateFixture(updateResults);
            FailFixtureInternal(error);
        }
    }

    /// <summary>
    /// Makes the current fixture broken and stops it. Removes all unfinished steps.
    /// </summary>
    /// <remarks>
    /// If no fixture is running, does nothing.
    /// </remarks>
    public static void BreakFixture()
    {
        if (HasFixture)
        {
            BreakFixtureInternal();
        }
    }

    /// <summary>
    /// Makes the current fixture broken and stops it. Removes all unfinished steps.
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// If no fixture is running, does nothing.
    /// </remarks>
    /// <param name="updateResults">
    /// The callback that is called before the fixture is stopped.
    /// </param>
    public static void BreakFixture(Action<FixtureResult> updateResults)
    {
        if (HasFixture)
        {
            CurrentLifecycle.UpdateFixture(updateResults);
            BreakFixtureInternal();
        }
    }

    /// <summary>
    /// Makes the current fixture broken and stops it. Removes all unfinished steps.
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// If no fixture is running, does nothing.
    /// </remarks>
    /// <param name="error">The error to report at the fixture level.</param>
    public static void BreakFixture(Exception error)
    {
        if (HasFixture)
        {
            BreakFixtureInternal(error);
        }
    }

    /// <summary>
    /// Makes the current fixture broken and stops it. Removes all unfinished steps.
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// If no fixture is running, does nothing.
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
        if (HasFixture)
        {
            CurrentLifecycle.UpdateFixture(updateResults);
            BreakFixtureInternal(error);
        }
    }

    /// <summary>
    /// Makes the current fixture skipped and stops it. Removes all unfinished steps.
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// If no fixture is running, does nothing.
    /// </remarks>
    public static void SkipFixture()
    {
        if (HasFixture)
        {
            SkipFixtureInternal();
        }
    }

    /// <summary>
    /// Makes the current fixture skipped and stops it. Removes all unfinished steps.
    /// the fixture contexts.
    /// </summary>
    /// <remarks>
    /// If no fixture is running, does nothing.
    /// </remarks>
    /// <param name="updateResults">
    /// The callback that is called before the fixture is stopped.
    /// </param>
    public static void SkipFixture(Action<FixtureResult> updateResults)
    {
        if (HasFixture)
        {
            CurrentLifecycle.UpdateFixture(updateResults);
            SkipFixtureInternal();
        }
    }

    /// <summary>
    /// Resolves the status of the current fixture to passed, failed, or broken
    /// depending on the exception and stops it. Removes all unfinished steps.
    /// </summary>
    /// <remarks>
    /// If no fixture is running, does nothing.
    /// </remarks>
    /// <param name="error">
    /// An exception instance. If it's <c>null</c>, the fixture becomes passed.
    /// Otherwise, the fixture becomes failed or broken depending on the
    /// exception's type and the current configuration.
    /// </param>
    public static void ResolveFixture(Exception? error)
    {
        if (HasFixture)
        {
            ResolveFixtureInternal(error);
        }
    }

    #endregion

    #region Low-level steps API

    /// <summary>
    /// Starts a new step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    public static void StartStep(string name)
    {
        if (HasTestOrFixture)
        {
            StartStepInternal(name);
        }
    }

    /// <summary>
    /// Starts a new step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="updateResults">
    /// The callback that is executed right after the step is started.
    /// </param>
    public static void StartStep(string name, Action<StepResult> updateResults)
    {
        if (HasTestOrFixture)
        {
            StartStepInternal(name);
            CurrentLifecycle.UpdateStep(updateResults);
        }
    }

    /// <summary>
    /// Makes the current step passed and stops it.
    /// </summary>
    /// <remarks>If no step is running, does nothing.</remarks>
    public static void PassStep()
    {
        if (HasStep)
        {
            PassStepInternal();
        }
    }

    /// <summary>
    /// Makes the current step passed and stops it.
    /// </summary>
    /// <remarks>If no step is running, does nothing.</remarks>
    /// <param name="updateResults">
    /// The callback that is executed before the step is stopped.
    /// </param>
    public static void PassStep(Action<StepResult> updateResults)
    {
        if (HasStep)
        {
            CurrentLifecycle.UpdateStep(updateResults);
            PassStepInternal();
        }
    }

    /// <summary>
    /// Makes the current step failed and stops it.
    /// </summary>
    /// <remarks>If no step is running, does nothing.</remarks>
    public static void FailStep()
    {
        if (HasStep)
        {
            FailStepInternal();
        }
    }

    /// <summary>
    /// Makes the current step failed and stops it.
    /// </summary>
    /// <remarks>If no step is running, does nothing.</remarks>
    /// <param name="updateResults">
    /// The callback that is executed before the step is stopped.
    /// </param>
    public static void FailStep(Action<StepResult> updateResults)
    {
        if (HasStep)
        {
            CurrentLifecycle.UpdateStep(updateResults);
            FailStepInternal();
        }
    }

    /// <summary>
    /// Makes the current step failed and stops it.
    /// </summary>
    /// <remarks>If no step is running, does nothing.</remarks>
    /// <param name="error">The error to report at the step level.</param>
    public static void FailStep(Exception error)
    {
        if (HasStep)
        {
            FailStepInternal(error);
        }
    }

    /// <summary>
    /// Makes the current step failed and stops it.
    /// </summary>
    /// <remarks>If no step is running, does nothing.</remarks>
    /// <param name="updateResults">
    /// The callback that is executed before the step is stopped.
    /// </param>
    /// <param name="error">The error to report at the step level.</param>
    public static void FailStep(
        Action<StepResult> updateResults,
        Exception error
    )
    {
        if (HasStep)
        {
            CurrentLifecycle.UpdateStep(updateResults);
            FailStepInternal(error);
        }
    }

    /// <summary>
    /// Makes the current step broken and stops it.
    /// </summary>
    /// <remarks>If no step is running, does nothing.</remarks>
    public static void BreakStep()
    {
        if (HasStep)
        {
            BreakStepInternal();
        }
    }

    /// <summary>
    /// Makes the current step broken and stops it.
    /// </summary>
    /// <remarks>If no step is running, does nothing.</remarks>
    /// <param name="updateResults">
    /// The callback that is executed before the step is stopped.
    /// </param>
    public static void BreakStep(Action<StepResult> updateResults)
    {
        if (HasStep)
        {
            CurrentLifecycle.UpdateStep(updateResults);
            BreakStepInternal();
        }
    }

    /// <summary>
    /// Makes the current step broken and stops it.
    /// </summary>
    /// <remarks>If no step is running, does nothing.</remarks>
    /// <param name="error">The error to report at the step level.</param>
    public static void BreakStep(Exception error)
    {
        if (HasStep)
        {
            BreakStepInternal(error);
        }
    }

    /// <summary>
    /// Makes the current step broken and stops it.
    /// </summary>
    /// <remarks>If no step is running, does nothing.</remarks>
    /// <param name="updateResults">
    /// The callback that is executed before the step is stopped.
    /// </param>
    /// <param name="error">The error to report at the step level.</param>
    public static void BreakStep(
        Action<StepResult> updateResults,
        Exception error
    )
    {
        if (HasStep)
        {
            CurrentLifecycle.UpdateStep(updateResults);
            BreakStepInternal(error);
        }
    }

    /// <summary>
    /// Makes the current step skipped and stops it.
    /// </summary>
    /// <remarks>If no step is running, does nothing.</remarks>
    public static void SkipStep()
    {
        if (HasStep)
        {
            SkipStepInternal();
        }
    }

    /// <summary>
    /// Makes the current step skipped and stops it.
    /// </summary>
    /// <remarks>If no step is running, does nothing.</remarks>
    /// <param name="updateResults">
    /// The callback that is executed before the step is stopped.
    /// </param>
    public static void SkipStep(Action<StepResult> updateResults)
    {
        if (HasStep)
        {
            CurrentLifecycle.UpdateStep(updateResults);
            SkipStepInternal();
        }
    }

    /// <summary>
    /// Resolves the status of the current step to passed, failed, or broken
    /// depending on the exception and stops it.
    /// </summary>
    /// <remarks>
    /// If no step is running, does nothing.
    /// </remarks>
    /// <param name="error">
    /// An exception instance. If it's <c>null</c>, the step becomes passed.
    /// Otherwise, the step becomes failed or broken depending on the
    /// exception's type and the current configuration.
    /// </param>
    public static void ResolveStep(Exception? error)
    {
        if (HasStep)
        {
            ResolveStepInternal(error);
        }
    }

    #endregion

    #region Lambda fixtures

    /// <summary>
    /// Executes the action and reports the result as a new setup fixture.
    /// </summary>
    /// <remarks>If Allure is not running, just calls the action.</remarks>
    /// <param name="name">The name of the setup fixture.</param>
    /// <param name="action">The code to run.</param>
    public static void Before(string name, Action action)
    {
        if (HasContainer)
        {
            BeforeInternal(name, () =>
            {
                action();
                return null as object;
            });
        }
        else
        {
            action();
        }
    }

    /// <summary>
    /// Executes the function and reports the result as a new setup fixture.
    /// </summary>
    /// <remarks>If Allure is not running, just calls the function.</remarks>
    /// <param name="name">The name of the setup fixture.</param>
    /// <param name="function">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static T Before<T>(string name, Func<T> function) =>
        HasContainer ? BeforeInternal(name, function) : function();

    /// <summary>
    /// Executes the asynchronous action and reports the result as a new setup
    /// fixture.
    /// </summary>
    /// <remarks>If Allure is not running, just calls the action.</remarks>
    /// <param name="name">The name of the setup fixture.</param>
    /// <param name="action">The asynchronous code to run.</param>
    public static async Task Before(string name, Func<Task> action)
    {
        if (HasContainer)
        {
            await ExecuteFixtureAsync(
                name,
                StartBeforeFixtureInternal,
                async () =>
                {
                    await action();
                    return Task.FromResult<object?>(null);
                }
            );
        }
        else
        {
            await action();
        }
    }

    /// <summary>
    /// Executes the asynchronous function and reports the result as a new
    /// setup fixture.
    /// </summary>
    /// <remarks>If Allure is not running, just calls the function.</remarks>
    /// <param name="name">The name of the setup fixture.</param>
    /// <param name="function">The asynchronous function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static async Task<T> Before<T>(string name, Func<Task<T>> function) =>
        await (HasContainer ? ExecuteFixtureAsync(
            name,
            StartBeforeFixtureInternal,
            function
        ) : function());

    /// <summary>
    /// Executes the action and reports the result as a new teardown fixture.
    /// </summary>
    /// <remarks>If Allure is not running, just calls the action.</remarks>
    /// <param name="name">The name of the teardown fixture.</param>
    /// <param name="action">The code to run.</param>
    public static void After(string name, Action action)
    {
        if (HasContainer)
        {
            AfterInternal(name, () =>
            {
                action();
                return null as object;
            });
        }
        else
        {
            action();
        }
    }

    /// <summary>
    /// Executes the function and reports the result as a new teardown fixture.
    /// </summary>
    /// <remarks>If Allure is not running, just calls the function.</remarks>
    /// <param name="name">The name of the teardown fixture.</param>
    /// <param name="function">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static T After<T>(string name, Func<T> function) =>
        HasContainer ? AfterInternal(name, function) : function();

    /// <summary>
    /// Executes the asynchronous action and reports the result as a new
    /// teardown fixture.
    /// </summary>
    /// <remarks>If Allure is not running, just calls the action.</remarks>
    /// <param name="name">The name of the teardown fixture.</param>
    /// <param name="action">The asynchronous code to run.</param>
    public static async Task After(string name, Func<Task> action)
    {
        if (HasContainer)
        {
            await ExecuteFixtureAsync(name, StartAfterFixtureInternal, async () =>
            {
                await action();
                return Task.FromResult<object?>(null);
            });
        }
        else
        {
            await action();
        }
    }

    /// <summary>
    /// Executes the asynchronous function and reports the result as a new
    /// teardown fixture.
    /// </summary>
    /// <remarks>If Allure is not running, just calls the function.</remarks>
    /// <param name="name">The name of the teardown fixture.</param>
    /// <param name="function">The asynchronous function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static async Task<T> After<T>(string name, Func<Task<T>> function) =>
        await (HasContainer ? ExecuteFixtureAsync(
            name,
            StartAfterFixtureInternal,
            function
        ) : function());

    #endregion

    static IEnumerable<string> FailExceptions
    {
        get => CurrentLifecycle.AllureConfiguration.FailExceptions
            ?? Enumerable.Empty<string>();
    }

    static void StartBeforeFixtureInternal(string name) =>
        CurrentLifecycle.StartBeforeFixture(new() { name = name });

    static void StartAfterFixtureInternal(string name) =>
        CurrentLifecycle.StartAfterFixture(new() { name = name });

    static void PassFixtureInternal() => CurrentLifecycle.StopFixture(result =>
    {
        result.status = Status.passed;
    });

    static void FailFixtureInternal() => CurrentLifecycle.StopFixture(result =>
    {
        result.status = Status.failed;
    });

    static void FailFixtureInternal(Exception error) =>
        CurrentLifecycle.StopFixture(result =>
        {
            result.status = Status.failed;
            result.statusDetails = ModelFunctions.ToStatusDetails(error);
        });

    static void BreakFixtureInternal() => CurrentLifecycle.StopFixture(result =>
    {
        result.status = Status.broken;
    });

    static void BreakFixtureInternal(Exception error) =>
        CurrentLifecycle.StopFixture(result =>
        {
            result.status = Status.broken;
            result.statusDetails = ModelFunctions.ToStatusDetails(error);
        });

    static void SkipFixtureInternal() => CurrentLifecycle.StopFixture(
        result => result.status = Status.skipped
    );

    static void ResolveFixtureInternal(Exception? error) =>
        ResolveItem(CurrentLifecycle.StopFixture, error);

    static void StartStepInternal(string name) =>
        CurrentLifecycle.StartStep(new() { name = name });

    static void PassStepInternal() => CurrentLifecycle.StopStep(result =>
    {
        result.status = Status.passed;
    });

    static void FailStepInternal() =>
        CurrentLifecycle.StopStep(result =>
        {
            result.status = Status.failed;
        });

    static void FailStepInternal(Exception error) =>
        CurrentLifecycle.StopStep(result =>
        {
            result.status = Status.failed;
            result.statusDetails = ModelFunctions.ToStatusDetails(error);
        });

    static void BreakStepInternal() => CurrentLifecycle.StopStep(result =>
    {
        result.status = Status.broken;
    });

    static void BreakStepInternal(Exception error) =>
        CurrentLifecycle.StopStep(result =>
        {
            result.status = Status.broken;
            result.statusDetails = ModelFunctions.ToStatusDetails(error);
        });

    static void SkipStepInternal() => CurrentLifecycle.StopStep(
        result => result.status = Status.skipped
    );

    static void ResolveStepInternal(Exception? error) =>
        ResolveItem(CurrentLifecycle.StopStep, error);

    static T BeforeInternal<T>(string name, Func<T> function) =>
        ExecuteFixture(name, StartBeforeFixtureInternal, function);

    static T AfterInternal<T>(string name, Func<T> function) =>
        ExecuteFixture(name, StartAfterFixtureInternal, function);

    static T ExecuteFixture<T>(
        string name,
        Action<string> start,
        Func<T> action
    ) =>
        AllureApi.ExecuteAction(
            name,
            start,
            action,
            resolve: ResolveFixtureInternal
        );

    static async Task<T> ExecuteFixtureAsync<T>(
        string name,
        Action<string> startFixture,
        Func<Task<T>> action
    ) =>
        await AllureApi.ExecuteActionAsync(
            () => startFixture(name),
            action,
            resolve: ResolveFixtureInternal
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
