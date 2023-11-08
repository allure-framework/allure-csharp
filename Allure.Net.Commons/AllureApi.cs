﻿using System;
using System.IO;
using System.Threading.Tasks;
using Allure.Net.Commons.Functions;
using Allure.Net.Commons.Steps;
using HeyRed.Mime;

#nullable enable

namespace Allure.Net.Commons;

/// <summary>
/// A facade that provides the API for test authors to enhance the Allure report.
/// </summary>
public static class AllureApi
{
    static AllureLifecycle? lifecycleInstance;

    internal static AllureLifecycle CurrentLifecycle
    {
        get => lifecycleInstance ?? AllureLifecycle.Instance;
        set => lifecycleInstance = value;
    }

    /// <summary>
    /// The logger that is notified about start and stop events of steps and
    /// fixtures.
    /// </summary>
    public static IStepLogger? StepLogger { get; set; }

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
        StepLogger?.BeforeStarted?.Log(name);
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
        StepLogger?.AfterStarted?.Log(name);
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
            StepLogger?.StepPassed?.Log(result.name);
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
            StepLogger?.StepFailed?.Log(result.name);
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
            StepLogger?.StepBroken?.Log(result.name);
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
        StepLogger?.StepStarted?.Log(name);
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
            StepLogger?.StepPassed?.Log(result.name);
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
            StepLogger?.StepFailed?.Log(result.name);
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
    /// Stops the current step making it broken. Requires the step context to
    /// be active.
    /// </summary>
    public static void BreakStep() => CurrentLifecycle.StopStep(
        result =>
        {
            result.status = Status.broken;
            StepLogger?.StepBroken?.Log(result.name);
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

    #endregion

    #region Noop step

    /// <summary>
    /// Adds an empty step to the current fixture, test or step. Requires one
    /// of these contexts to be active.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    public static void Step(string name) =>
        Step(name, () => { });

    #endregion

    #region Lambda steps

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step. Requires one of these contexts to be active.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="action">The code to run.</param>
    public static void Step(string name, Action action)
    {
        ExecuteStep(name, () =>
        {
            action();
            return null as object;
        });
    }

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step. Requires one of these contexts to be
    /// active.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="function">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static T Step<T>(string name, Func<T> function) =>
        ExecuteStep(name, function);

    /// <summary>
    /// Executes the asynchronous action and reports the result as a new step
    /// of the current fixture, test or step. Requires one of these contexts to
    /// be active.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="action">The asynchronous code to run.</param>
    public static async Task Step(string name, Func<Task> action) =>
        await ExecuteStepAsync(name, async () =>
        {
            await action();
            return Task.FromResult<object?>(null);
        });

    /// <summary>
    /// Executes the asynchronous function and reports the result as a new step
    /// of the current fixture, test or step. Requires one of these contexts to
    /// be active.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="function">The asynchronous function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static async Task<T> Step<T>(string name, Func<Task<T>> function) =>
        await ExecuteStepAsync(name, function);

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

    #region Attachments

    // TODO: read file in background thread
    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// Requires one of those contexts to be active.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="type">The MIME type of the attachment.</param>
    /// <param name="path">The path to the attached file.</param>
    public static void AddAttachment(
        string name,
        string type,
        string path
    ) =>
        AddAttachment(
            name: name,
            type: type,
            content: File.ReadAllBytes(path),
            fileExtension: new FileInfo(path).Extension
        );

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// Requires one of those contexts to be active.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="type">The MIME type of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="fileExtension">
    /// The extension of the file that will be available for downloading.
    /// </param>
    public static void AddAttachment(
        string name,
        string type,
        byte[] content,
        string fileExtension = ""
    )
    {
        var suffix = AllureConstants.ATTACHMENT_FILE_SUFFIX;
        var uuid = IdFunctions.CreateUUID();
        var source = $"{uuid}{suffix}{fileExtension}";
        var attachment = new Attachment
        {
            name = name,
            type = type,
            source = source
        };
        CurrentLifecycle.Writer.Write(source, content);
        CurrentLifecycle.UpdateExecutableItem(
            item => item.attachments.Add(attachment)
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// Requires one of those contexts to be active.
    /// </summary>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">
    /// The name of the attachment. If null, the file name is used.
    /// </param>
    public static void AddAttachment(
        string path,
        string? name = null
    ) =>
        AddAttachment(
            name: name ?? Path.GetFileName(path),
            type: MimeTypesMap.GetMimeType(path),
            path: path
        );

    /// <summary>
    /// Attaches screen diff images to the current test case.
    /// </summary>
    /// <remarks>
    /// Requires the test, the fixture, or the step context to be active.
    /// </remarks>
    /// <param name="expectedPng">A path to the actual screen.</param>
    /// <param name="actualPng">A path to the expected screen.</param>
    /// <param name="diffPng">A path to the screen diff.</param>
    /// <exception cref="InvalidOperationException"/>
    public static void AddScreenDiff(
        string expectedPng,
        string actualPng,
        string diffPng
    )
    {
        AddAttachment(expectedPng, "expected");
        AddAttachment(actualPng, "actual");
        AddAttachment(diffPng, "diff");
        CurrentLifecycle.UpdateTestCase(
            x => x.labels.Add(Label.TestType("screenshotDiff"))
        );
    }

    #endregion

    static T ExecuteStep<T>(string name, Func<T> action) =>
        ExecuteAction(
            name,
            StartStep,
            action,
            PassStep,
            FailStep
        );

    static async Task<T> ExecuteStepAsync<T>(
        string name,
        Func<Task<T>> action
    ) =>
        await ExecuteActionAsync(
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

    static async Task<T> ExecuteFixtureAsync<T>(
        string name,
        Action<string> startFixture,
        Func<Task<T>> action
    ) =>
        await ExecuteActionAsync(
            () => startFixture(name),
            action,
            PassFixture,
            FailFixture
        );

    static async Task<T> ExecuteActionAsync<T>(
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

    static T ExecuteAction<T>(
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
}