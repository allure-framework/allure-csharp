using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Allure.Net.Commons.Functions;
using HeyRed.Mime;
using Newtonsoft.Json;

#nullable enable

namespace Allure.Net.Commons;

/// <summary>
/// A facade that provides the API for test authors to enhance the Allure
/// report.
/// </summary>
public static class AllureApi
{
    const string DIFF_NAME_PATTERN = "diff-{0}";
    const string DIFF_MEDIA_TYPE = "application/vnd.allure.image.diff";
    const string DIFF_ENTRY_PREFIX = "data:image/png;base64,";

    static AllureLifecycle? lifecycleInstance;

    internal static AllureLifecycle CurrentLifecycle
    {
        get => lifecycleInstance ?? AllureLifecycle.Instance;
        set => lifecycleInstance = value;
    }

    internal static bool HasTest => ExtendedApi.HasTest;
    internal static bool HasFixture => ExtendedApi.HasFixture;
    internal static bool HasStep => ExtendedApi.HasStep;
    internal static bool HasTestOrFixture => ExtendedApi.HasTestOrFixture;

    #region Metadata

    /// <summary>
    /// Sets the name of the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="newName">The new name of the test.</param>
    public static void SetTestName(string newName)
    {
        if (HasTest)
        {
            CurrentLifecycle.UpdateTestCase(t => t.name = newName);
        }
    }

    /// <summary>
    /// Sets the name of the current fixture.
    /// </summary>
    /// <remarks>If no fixture is running, does nothing.</remarks>
    /// <param name="newName">The new name of the fixture.</param>
    public static void SetFixtureName(string newName)
    {
        if (HasFixture)
        {
            CurrentLifecycle.UpdateFixture(f => f.name = newName);
        }
    }

    /// <summary>
    /// Sets the name of the current step.
    /// </summary>
    /// <remarks>If no step is running, does nothing.</remarks>
    /// <param name="newName">The new name of the step.</param>
    public static void SetStepName(string newName)
    {
        if (HasStep)
        {
            CurrentLifecycle.UpdateStep(s => s.name = newName);
        }
    }

    /// <summary>
    /// Sets the description of the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="description">The description of the test.</param>
    public static void SetDescription(string description)
    {
        if (HasTest)
        {
            CurrentLifecycle.UpdateTestCase(tr => tr.description = description);
        }
    }

    /// <summary>
    /// Sets the description of the current test. Allows HTML to be used.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="descriptionHtml">
    /// The description in the HTML format.
    /// </param>
    public static void SetDescriptionHtml(string descriptionHtml)
    {
        if (HasTest)
        {
            CurrentLifecycle.UpdateTestCase(tr => tr.descriptionHtml = descriptionHtml);
        }
    }

    /// <summary>
    /// Adds new labels to the test's list of labels.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="labels">The labels to add.</param>
    public static void AddLabels(params Label[] labels)
    {
        if (HasTest)
        {
            CurrentLifecycle.UpdateTestCase(tr => tr.labels.AddRange(labels));
        }
    }

    /// <summary>
    /// Adds a new label to the current test result.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the label to add.</param>
    /// <param name="value">The value of the label to add.</param>
    public static void AddLabel(string name, string value) =>
        AddLabel(new() { name = name, value = value });

    /// <summary>
    /// Adds a label to the current test result.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="label">The new label of the test.</param>
    public static void AddLabel(Label label)
    {
        if (HasTest)
        {
            CurrentLifecycle.UpdateTestCase(tr => tr.labels.Add(label));
        }
    }

    /// <summary>
    /// Sets the current test's severity.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="severity">The new severity level of the test.</param>
    public static void SetSeverity(SeverityLevel severity)
    {
        if (HasTest)
        {
            SetLabel(
                Label.Severity(severity)
            );
        }
    }

    /// <summary>
    /// Sets the current test's owner.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="owner">The new owner of the test.</param>
    public static void SetOwner(string owner)
    {
        if (HasTest)
        {
            SetLabel(
                Label.Owner(owner)
            );
        }
    }

    /// <summary>
    /// Sets the current test's ID.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="allureId">The new ID of the test case.</param>
    public static void SetAllureId(int allureId)
    {
        if (HasTest)
        {
            SetLabel(
                Label.AllureId(allureId)
            );
        }
    }

    /// <summary>
    /// Adds tags to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
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
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="parentSuite">The parent suite to be added.</param>
    public static void AddParentSuite(string parentSuite) =>
        AddLabel(
            Label.ParentSuite(parentSuite)
        );

    /// <summary>
    /// Adds an additional suite to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="suite">The suite to be added.</param>
    public static void AddSuite(string suite) =>
        AddLabel(
            Label.Suite(suite)
        );

    /// <summary>
    /// Adds an additional sub-suite to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="subSuite">The sub-suite to be added.</param>
    public static void AddSubSuite(string subSuite) =>
        AddLabel(
            Label.SubSuite(subSuite)
        );

    #endregion

    #region BDD-labels

    /// <summary>
    /// Adds an additional epic to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="epic">The epic to be added.</param>
    public static void AddEpic(string epic) =>
        AddLabel(
            Label.Epic(epic)
        );

    /// <summary>
    /// Adds an additional feature to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="feature">The feature to be added.</param>
    public static void AddFeature(string feature) =>
        AddLabel(
            Label.Feature(feature)
        );

    /// <summary>
    /// Adds an additional story to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="story">The story to be added.</param>
    public static void AddStory(string story) =>
        AddLabel(
            Label.Story(story)
        );

    #endregion

    #region Links

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The address of the link.</param>
    public static void AddLink(string url) =>
        AddLinks(
            new Link { url = url }
        );

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The display text of the link.</param>
    /// <param name="url">The address of the link.</param>
    public static void AddLink(string name, string url) =>
        AddLinks(
            new Link { name = name, url = url }
        );

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
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
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="links">The link instances to add.</param>
    public static void AddLinks(params Link[] links)
    {
        if (HasTest)
        {
            CurrentLifecycle.UpdateTestCase(t => t.links.AddRange(links));
        }
    }

    /// <summary>
    /// Adds a new issue link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The URL of the issue.</param>
    public static void AddIssue(string url) =>
        AddLinks(
            new Link { type = LinkType.ISSUE, url = url }
        );

    /// <summary>
    /// Adds a new issue link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The display text of the issue link.</param>
    /// <param name="url">The URL of the issue.</param>
    public static void AddIssue(string name, string url) =>
        AddLink(name, LinkType.ISSUE, url);

    /// <summary>
    /// Adds a new TMS item link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The URL of the TMS item.</param>
    public static void AddTmsItem(string url) =>
        AddLinks(
            new Link { type = LinkType.TMS_ITEM, url = url }
        );

    /// <summary>
    /// Adds a new TMS item link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The display text of the TMS item link.</param>
    /// <param name="url">The URL of the TMS item.</param>
    public static void AddTmsItem(string name, string url) =>
        AddLink(name, LinkType.TMS_ITEM, url);

    #endregion

    #region Noop step

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    public static void Step(string name)
    {
        if (HasTestOrFixture)
        {
            Step(name, () => { });
        }
    }

    #endregion

    #region Lambda steps

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <remarks>
    /// If no test or fixture is running, just calls the action.
    /// </remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="action">The code to run.</param>
    public static void Step(string name, Action action)
    {
        if (HasTestOrFixture)
        {
            ExecuteStep(name, () =>
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
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <remarks>
    /// If no test or fixture is running, just calls the function and returns
    /// its result.
    /// </remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="function">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static T Step<T>(string name, Func<T> function) =>
        HasTestOrFixture ? ExecuteStep(name, function) : function();

    /// <summary>
    /// Executes the asynchronous action and reports the result as a new step
    /// of the current fixture, test or step.
    /// </summary>
    /// <remarks>
    /// If no test or fixture is running, just awaits the action.
    /// </remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="action">The asynchronous code to run.</param>
    public static async Task Step(string name, Func<Task> action)
    {
        if (HasTestOrFixture)
        {
            await ExecuteStepAsync(name, async () =>
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
    /// Executes the asynchronous function and reports the result as a new step
    /// of the current fixture, test or step.
    /// </summary>
    /// <remarks>
    /// If no test or fixture is running, just awaits the function and returns
    /// its result.
    /// </remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="function">The asynchronous function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static async Task<T> Step<T>(string name, Func<Task<T>> function) =>
        await (HasTestOrFixture ? ExecuteStepAsync(name, function) : function());

    #endregion

    #region Attachments

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="type">The MIME type of the attachment.</param>
    /// <param name="path">The path to the attached file.</param>
    public static void AddAttachment(
        string name,
        string type,
        string path
    )
    {
        if (HasTestOrFixture)
        {
            AddAttachmentInternal(
                name: name ?? Path.GetFileName(path),
                type: type,
                content: File.ReadAllBytes(path),
                fileExtension: Path.GetExtension(path)
            );
        }
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
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
        if (HasTestOrFixture)
        {
            AddAttachmentInternal(name, type, content, fileExtension);
        }
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">
    /// The name of the attachment. If null, the file name is used.
    /// </param>
    public static void AddAttachment(
        string path,
        string? name = null
    )
    {
        if (HasTestOrFixture)
        {
            AddAttachmentInternal(
                name: name ?? Path.GetFileName(path),
                type: MimeTypesMap.GetMimeType(path),
                content: File.ReadAllBytes(path),
                fileExtension: Path.GetExtension(path)
            );
        }
    }

    /// <summary>
    /// Attaches screen diff images to the current fixture, test, or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="expectedPngPath">A path to the actual screen.</param>
    /// <param name="actualPngPath">A path to the expected screen.</param>
    /// <param name="diffPngPath">A path to the screen diff.</param>
    public static void AddScreenDiff(
        string expectedPngPath,
        string actualPngPath,
        string diffPngPath
    )
    {
        if (HasTestOrFixture)
        {
            AddScreenDiffInternal(
                File.ReadAllBytes(expectedPngPath),
                File.ReadAllBytes(actualPngPath),
                File.ReadAllBytes(diffPngPath)
            );
        }
    }

    /// <summary>
    /// Attaches screen diff images to the current fixture, test, or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="expectedPng">An actual screen bytes.</param>
    /// <param name="actualPng">An expected screen bytes.</param>
    /// <param name="diffPng">A screen diff bytes.</param>
    public static void AddScreenDiff(
        byte[] expectedPng,
        byte[] actualPng,
        byte[] diffPng
    )
    {
        if (HasTestOrFixture)
        {
            AddScreenDiffInternal(expectedPng, actualPng, diffPng);
        }
    }

    #endregion

    #region Parameters

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter. The value is converted to a string
    /// using JSON serialization. Use <see cref="AddTestParameter(Parameter)"/>
    /// or add a suitable type formatter via
    /// <see cref="AllureLifecycle.AddTypeFormatter{T}(TypeFormatter{T})"/> to
    /// customize the serialization.
    /// </param>
    public static void AddTestParameter(string name, object? value)
    {
        if (HasTest)
        {
            AddTestParameter(new()
            {
                name = name,
                value = FormatParameterValue(value),
            });
        }
    }

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter. The value is converted to a string
    /// using JSON serialization. Use <see cref="AddTestParameter(Parameter)"/>
    /// or add a suitable type formatter via
    /// <see cref="AllureLifecycle.AddTypeFormatter{T}(TypeFormatter{T})"/> to
    /// customize the serialization.
    /// </param>
    /// <param name="mode">The display mode of the new parameter.</param>
    public static void AddTestParameter(string name, object? value, ParameterMode mode)
    {
        if (HasTest)
        {
            AddTestParameter(new()
            {
                name = name,
                value = FormatParameterValue(value),
                mode = mode,
            });
        }
    }

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter. The value is converted to a string
    /// using JSON serialization. Use <see cref="AddTestParameter(Parameter)"/>
    /// or add a suitable type formatter via
    /// <see cref="AllureLifecycle.AddTypeFormatter{T}(TypeFormatter{T})"/> to
    /// customize the serialization.
    /// </param>
    /// <param name="excluded">
    /// The exclusion flag of the new parameter. If set to true, the parameter
    /// doesn't affect the test's history.
    /// </param>
    public static void AddTestParameter(string name, object? value, bool excluded)
    {
        if (HasTest)
        {
            AddTestParameterInternal(new()
            {
                name = name,
                value = FormatParameterValue(value),
                excluded = excluded
            });
        }
    }

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter. The value is converted to a string
    /// using JSON serialization. Use <see cref="AddTestParameter(Parameter)"/>
    /// or add a suitable type formatter via
    /// <see cref="AllureLifecycle.AddTypeFormatter{T}(TypeFormatter{T})"/> to
    /// customize the serialization.
    /// </param>
    /// <param name="mode">The display mode of the new parameter.</param>
    /// <param name="excluded">
    /// The exclusion flag of the new parameter. If set to true, the parameter
    /// doesn't affect the test's history.
    /// </param>
    public static void AddTestParameter(
        string name,
        object? value,
        ParameterMode mode,
        bool excluded
    )
    {
        if (HasTest)
        {
            AddTestParameterInternal(new()
            {
                name = name,
                value = FormatParameterValue(value),
                mode = mode,
                excluded = excluded
            });
        }
    }

    /// <summary>
    /// Adds a new parameter to the current test. Use this overload if you
    /// want to manually control how the parameter's value should be displayed
    /// in the report.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="parameter">
    /// A new parameter instance.
    /// </param>
    public static void AddTestParameter(Parameter parameter)
    {
        if (HasTest)
        {
            AddTestParameterInternal(parameter);
        }
    }

    static void AddTestParameterInternal(Parameter parameter)
    {
        CurrentLifecycle.UpdateTestCase(
            t => t.parameters.Add(parameter)
        );
    }

    #endregion

    static void SetLabel(Label label) =>
        CurrentLifecycle.UpdateTestCase(tr =>
        {
            tr.labels.RemoveAll(lr => lr.name == label.name);
            tr.labels.Add(label);
        });

    static string FormatParameterValue(object? value) =>
        FormatFunctions.Format(value, CurrentLifecycle.TypeFormatters);

    static T ExecuteStep<T>(string name, Func<T> action) =>
        ExecuteAction(
            name,
            ExtendedApi.StartStep,
            action,
            ExtendedApi.ResolveStep
        );

    static async Task<T> ExecuteStepAsync<T>(
        string name,
        Func<Task<T>> action
    ) =>
        await ExecuteActionAsync(
            () => ExtendedApi.StartStep(name),
            action,
            ExtendedApi.ResolveStep
        );

    internal static async Task<T> ExecuteActionAsync<T>(
        Action start,
        Func<Task<T>> action,
        Action<Exception?> resolve
    )
    {
        T result;
        Exception? error = null;
        start();
        try
        {
            result = await action();
        }
        catch (Exception e)
        {
            error = e;
            throw;
        }
        finally
        {
            resolve(error);
        }
        return result;
    }

    internal static T ExecuteAction<T>(
        string name,
        Action<string> start,
        Func<T> action,
        Action<Exception?> resolve
    )
    {
        T result;
        Exception? error = null;
        start(name);
        try
        {
            result = action();
        }
        catch (Exception e)
        {
            error = e;
            throw;
        }
        finally
        {
            resolve(error);
        }
        return result;
    }


    static void AddAttachmentInternal(
        string name,
        string type,
        byte[] content,
        string fileExtension
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

    static void AddScreenDiffInternal(
        byte[] expectedPng,
        byte[] actualPng,
        byte[] diffPng
    ) =>
        AddAttachment(
            string.Format(
                DIFF_NAME_PATTERN,
                CurrentLifecycle.Context.CurrentStepContainer.attachments.Count(
                    a => a.type == DIFF_MEDIA_TYPE
                ) + 1
            ),
            DIFF_MEDIA_TYPE,
            Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(new
                {
                    expected = ToDiffEntry(expectedPng),
                    actual = ToDiffEntry(actualPng),
                    diff = ToDiffEntry(diffPng)
                })
            ),
            ".json"
        );

    static string ToDiffEntry(byte[] data) =>
        DIFF_ENTRY_PREFIX + Convert.ToBase64String(data);
}
