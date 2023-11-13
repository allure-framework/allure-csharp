using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Allure.Net.Commons.Functions;
using Allure.Net.Commons.Steps;
using HeyRed.Mime;

#nullable enable

namespace Allure.Net.Commons;

/// <summary>
/// A facade that provides the API for test authors to enhance the Allure
/// report.
/// </summary>
public static class AllureApi
{
    static AllureLifecycle? lifecycleInstance;

    internal static AllureLifecycle CurrentLifecycle
    {
        get => lifecycleInstance ?? AllureLifecycle.Instance;
        set => lifecycleInstance = value;
    }

    #region Metadata

    /// <summary>
    /// Sets the name of the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="newName">The new name of the test.</param>
    public static void SetTestName(string newName) =>
        CurrentLifecycle.UpdateTestCase(t => t.name = newName);

    /// <summary>
    /// Sets the name of the current fixture.
    /// </summary>
    /// <remarks>Requires the fixture context to be active.</remarks>
    /// <param name="newName">The new name of the fixture.</param>
    public static void SetFixtureName(string newName) =>
        CurrentLifecycle.UpdateFixture(f => f.name = newName);

    /// <summary>
    /// Sets the name of the current step.
    /// </summary>
    /// <remarks>Requires the step context to be active.</remarks>
    /// <param name="newName">The new name of the step.</param>
    public static void SetStepName(string newName) =>
        CurrentLifecycle.UpdateStep(s => s.name = newName);

    /// <summary>
    /// Sets the description of the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="description">The description of the test.</param>
    public static void SetDescription(string description) =>
        CurrentLifecycle.UpdateTestCase(tr => tr.description = description);

    /// <summary>
    /// Sets the description of the current test. Allows HTML to be used.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="descriptionHtml">
    /// The description in the HTML format.
    /// </param>
    public static void SetDescriptionHtml(string descriptionHtml) =>
        CurrentLifecycle.UpdateTestCase(tr => tr.descriptionHtml = descriptionHtml);

    /// <summary>
    /// Adds new labels to the test's list of labels.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="labels">The labels to add.</param>
    public static void AddLabels(params Label[] labels) =>
        CurrentLifecycle.UpdateTestCase(tr => tr.labels.AddRange(labels));

    /// <summary>
    /// Adds a new label to the current test result.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="name">The name of the label to add.</param>
    /// <param name="value">The value of the label to add.</param>
    public static void AddLabel(string name, string value) =>
        AddLabel(new() { name = name, value = value });

    /// <summary>
    /// Adds a label to the current test result. Removes all previously added
    /// labels with the same name.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="name">The name of the label.</param>
    /// <param name="value">The value of the label.</param>
    public static void SetLabel(string name, string value) =>
        SetLabel(new() { name = name, value = value });

    /// <summary>
    /// Adds a label to the current test result.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="newLabel">The new label of the test.</param>
    public static void AddLabel(Label newLabel) =>
        CurrentLifecycle.UpdateTestCase(tr => tr.labels.Add(newLabel));

    /// <summary>
    /// Adds a label to the current test result. Removes all previously added
    /// labels with the same name.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="label">The new label of the test.</param>
    public static void SetLabel(Label label) =>
        CurrentLifecycle.UpdateTestCase(tr =>
        {
            tr.labels.RemoveAll(lr => lr.name == label.name);
            tr.labels.Add(label);
        });

    /// <summary>
    /// Sets the current test's severity.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="severity">The new severity level of the test.</param>
    public static void SetSeverity(SeverityLevel severity) =>
        SetLabel(
            Label.Severity(severity)
        );

    /// <summary>
    /// Sets the current test's owner.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="owner">The new owner of the test.</param>
    public static void SetOwner(string owner) =>
        SetLabel(
            Label.Owner(owner)
        );

    /// <summary>
    /// Sets the current test's ID.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="allureId">The new ID of the test case.</param>
    public static void SetAllureId(int allureId) =>
        SetLabel(
            Label.AllureId(allureId)
        );

    /// <summary>
    /// Adds tags to the current test.
    /// </summary>
    /// <param name="tags">The new tags.</param>
    public static void AddTags(params string[] tags) =>
        AddLabels(
            tags.Select(Label.Tag).ToArray()
        );

    #endregion

    #region Suites

    /// <summary>
    /// Adds an additional parent suite to the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="additionalParentSuite">The parent suite to be added.</param>
    public static void AddParentSuite(string additionalParentSuite) =>
        AddLabel(
            Label.ParentSuite(additionalParentSuite)
        );

    /// <summary>
    /// Sets the parent suite of the current test. Existing parent suites
    /// will be removed.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="newParentSuite">The new parent suite.</param>
    public static void SetParentSuite(string newParentSuite) =>
        SetLabel(
            Label.ParentSuite(newParentSuite)
        );

    /// <summary>
    /// Adds an additional suite to the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="additionalSuite">The suite to be added.</param>
    public static void AddSuite(string additionalSuite) =>
        AddLabel(
            Label.Suite(additionalSuite)
        );

    /// <summary>
    /// Sets the suite of the current test. Existing suites will be
    /// removed.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="newSuite">The new suite.</param>
    public static void SetSuite(string newSuite) =>
        SetLabel(
            Label.Suite(newSuite)
        );

    /// <summary>
    /// Adds an additional sub-suite to the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="additionalSubSuite">The sub-suite to be added.</param>
    public static void AddSubSuite(string additionalSubSuite) =>
        AddLabel(
            Label.SubSuite(additionalSubSuite)
        );

    /// <summary>
    /// Sets the sub-suite of the current test. Existing sub-suites will be
    /// removed.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="newSubSuite">The new sub-suite.</param>
    public static void SetSubSuite(string newSubSuite) =>
        SetLabel(
            Label.SubSuite(newSubSuite)
        );

    #endregion

    #region BDD-labels

    /// <summary>
    /// Adds an additional epic to the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="additionalEpic">The epic to be added.</param>
    public static void AddEpic(string additionalEpic) =>
        AddLabel(
            Label.Epic(additionalEpic)
        );

    /// <summary>
    /// Sets the epic of the current test. Existing epics will be removed.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="newEpic">The new epic.</param>
    public static void SetEpic(string newEpic) =>
        SetLabel(
            Label.Epic(newEpic)
        );

    /// <summary>
    /// Adds an additional feature to the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="additionalFeature">The feature to be added.</param>
    public static void AddFeature(string additionalFeature) =>
        AddLabel(
            Label.Feature(additionalFeature)
        );

    /// <summary>
    /// Sets the feature of the current test. Existing features will be removed.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="newFeature">The new feature.</param>
    public static void SetFeature(string newFeature) =>
        SetLabel(
            Label.Feature(newFeature)
        );

    /// <summary>
    /// Adds an additional story to the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="additionalStory">The story to be added.</param>
    public static void AddStory(string additionalStory) =>
        AddLabel(
            Label.Story(additionalStory)
        );

    /// <summary>
    /// Sets the story of the current test. Existing stories will be removed.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="newStory">The new story.</param>
    public static void SetStory(string newStory) =>
        SetLabel(
            Label.Story(newStory)
        );

    #endregion

    #region Links

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="url">The address of the link.</param>
    public static void AddLink(string url) =>
        AddLinks(
            new Link { url = url }
        );

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="name">The display text of the link.</param>
    /// <param name="url">The address of the link.</param>
    public static void AddLink(string name, string url) =>
        AddLinks(
            new Link { name = name, url = url }
        );

    /// <summary>
    /// Adds a new issue link to the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="url">The URL of the issue.</param>
    public static void AddIssue(string url) =>
        AddLinks(
            new Link { type = LinkType.ISSUE, url = url }
        );

    /// <summary>
    /// Adds a new issue link to the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="name">The display text of the issue link.</param>
    /// <param name="url">The URL of the issue.</param>
    public static void AddIssue(string name, string url) =>
        AddLink(name, LinkType.ISSUE, url);

    /// <summary>
    /// Adds a new TMS item link to the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="url">The URL of the TMS item.</param>
    public static void AddTmsItem(string url) =>
        AddLinks(
            new Link { type = LinkType.TMS_ITEM, url = url }
        );

    /// <summary>
    /// Adds a new TMS item link to the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="name">The display text of the TMS item link.</param>
    /// <param name="url">The URL of the TMS item.</param>
    public static void AddTmsItem(string name, string url) =>
        AddLink(name, LinkType.TMS_ITEM, url);

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="name">The display text of the link.</param>
    /// <param name="type">
    /// The type of the link. Used when matching link patterns. Might also
    /// affect how the link is rendered in the report.
    /// </param>
    /// <param name="url">The address of the link.</param>
    public static void AddLink(string name, string type, string url) =>
        AddLinks(
            new Link { name = name, type = type, url = url }
        );

    /// <summary>
    /// Adds new links to the current test.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="links">The link instances to add.</param>
    public static void AddLinks(params Link[] links) =>
        CurrentLifecycle.UpdateTestCase(t => t.links.AddRange(links));

    #endregion

    #region Steps and fixtures

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

    #region Parameters

    /// <summary>
    /// Adds a new test parameter or updates the value of the existing
    /// parameter.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="name">The name of a new or existing parameter.</param>
    /// <param name="value">
    /// The value of the new parameter, or the new value of the existing
    /// parameter. The value is converted to the string using JSON
    /// serialization. Use <see cref="SetTestParameter(Parameter)"/> or add a
    /// suitable type formatter to customize the serialization.
    /// </param>
    public static void SetTestParameter(string name, object? value) =>
        SetTestParameter(
            name,
            p => p.value = GetParameterValue(value),
            value,
            null,
            false
        );

    /// <summary>
    /// Adds a new test parameter or updates the value and the display mode
    /// of the existing parameter.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="name">The name of the new or existing parameter.</param>
    /// <param name="value">
    /// The value of the new parameter, or the new value of the existing
    /// parameter. The value is converted to the string using JSON
    /// serialization. Use <see cref="SetTestParameter(Parameter)"/> or add a
    /// suitable type formatter to customize the serialization.
    /// </param>
    /// <param name="mode">
    /// The display mode of the new parameter, or the new display mode of the
    /// existing parameter.
    /// </param>
    public static void SetTestParameter(string name, object? value, ParameterMode mode) =>
        SetTestParameter(
            name,
            p =>
            {
                p.value = GetParameterValue(value);
                p.mode = mode;
            },
            value,
            mode,
            false
        );

    /// <summary>
    /// Adds a new test parameter or updates the value and the exclusion flag
    /// of the existing parameter.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="name">The name of the new or existing parameter.</param>
    /// <param name="value">
    /// The value of the new parameter, or the new value of the existing
    /// parameter. The value is converted to the string using JSON
    /// serialization. Use <see cref="SetTestParameter(Parameter)"/> or add a
    /// suitable type formatter to customize the serialization.
    /// </param>
    /// <param name="excluded">
    /// The exclusion flag of the new parameter, or the new exclusion flag of
    /// the existing parameter.
    /// </param>
    public static void SetTestParameter(string name, object? value, bool excluded) =>
        SetTestParameter(
            name,
            p =>
            {
                p.value = GetParameterValue(value);
                p.excluded = excluded;
            },
            value,
            null,
            excluded
        );

    /// <summary>
    /// Adds a new test parameter or updates all properties of the existing
    /// parameter.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="name">The name of the new or existing parameter.</param>
    /// <param name="value">
    /// The value of the new parameter, or the new value of the existing
    /// parameter. The value is converted to the string using JSON
    /// serialization. Use <see cref="SetTestParameter(Parameter)"/> or add a
    /// suitable type formatter to customize the serialization.
    /// </param>
    /// <param name="mode">
    /// The display mode of the new parameter, or the new display mode of the
    /// existing parameter.
    /// </param>
    /// <param name="excluded">
    /// The exclusion flag of the new parameter, or the new exclusion flag of
    /// the existing parameter.
    /// </param>
    public static void SetTestParameter(
        string name,
        object? value,
        ParameterMode? mode,
        bool excluded
    ) =>
        SetTestParameter(new()
        {
            name = name,
            value = GetParameterValue(value),
            mode = mode,
            excluded = excluded
        });

    /// <summary>
    /// Adds or replaced a test parameter.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="parameter">
    /// A new parameter. If the parameter with the same name already exists,
    /// it's removed first.
    /// </param>
    public static void SetTestParameter(Parameter parameter)
    {
        CurrentLifecycle.UpdateTestCase(
            t =>
            {
                t.parameters.RemoveAll(p => p.name == parameter.name);
                t.parameters.Add(parameter);
            }
        );
    }

    /// <summary>
    /// Updates the existing test parameter. Throws, if the paramter doesn't
    /// exist in the test context.
    /// </summary>
    /// <remarks>Requires the test context to be active.</remarks>
    /// <param name="name">The name of the parameter to update.</param>
    /// <param name="mode">The new display mode of the parameter.</param>
    /// <param name="excluded">The new exclusion flag of the parameter.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void UpdateTestParameter(string name, ParameterMode? mode = null, bool? excluded = null) =>
        CurrentLifecycle.UpdateTestCase(t =>
        {
            var parameter = t.parameters.FirstOrDefault(p => p.name == name);
            if (parameter is null)
            {
                throw new InvalidOperationException(
                    $"The parameter '{name}' doesn't exist in the current test."
                );
            }

            if (mode is not null)
            {
                parameter.mode = mode.Value;
            }

            if (excluded is not null)
            {
                parameter.excluded = excluded.Value;
            }

        });

    #endregion

    static void SetTestParameter(
        string name,
        Action<Parameter> updateExistingParameter,
        object? value,
        ParameterMode? mode,
        bool excluded
    ) =>
        CurrentLifecycle.UpdateTestCase(t =>
        {
            if (t.parameters.FirstOrDefault(p => p.name == name) is Parameter parameter)
            {
                updateExistingParameter(parameter);
            }
            else
            {
                t.parameters.Add(new()
                {
                    name = name,
                    value = FormatFunctions.Format(value, CurrentLifecycle.TypeFormatters),
                    mode = mode,
                    excluded = excluded
                });
            }
        });

    static string GetParameterValue(object? value) =>
        FormatFunctions.Format(value, CurrentLifecycle.TypeFormatters);

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
