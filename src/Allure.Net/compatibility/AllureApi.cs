using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Allure.Model;

using NewAllureApi = Allure.AllureApi;

namespace Allure.Net.Commons;

/// <summary>
/// This class is a part of the legacy API compatibility layer and will be
/// removed in a future update.
/// Please, switch to <see cref="Allure.AllureApi"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
[Obsolete("Allure.Net.Commons.AllureApi is a legacy API and will be removed in a future update. Please, switch to Allure.AllureApi.")]
public static class AllureApi
{
    /// <summary>
    /// Sets the name of the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.SetTestName(string)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.SetTestName instead.")]
    public static void SetTestName(string newName)
    {
        NewAllureApi.SetTestName(newName);
    }

    /// <summary>
    /// Sets the name of the current fixture.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.SetFixtureName(string)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.SetFixtureName instead.")]
    public static void SetFixtureName(string newName)
    {
        NewAllureApi.SetFixtureName(newName);
    }

    /// <summary>
    /// Sets the name of the current step.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.SetName(string)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.SetName or the SetName method of a step context instead.")]
    public static void SetStepName(string newName)
    {
        NewAllureApi.SetName(newName);
    }

    /// <summary>
    /// Sets the description of the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.SetDescription(string)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.SetDescription instead.")]
    public static void SetDescription(string description)
    {
        NewAllureApi.SetDescription(description);
    }

    /// <summary>
    /// Sets the description of the current test. Allows HTML to be used.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.SetDescriptionHtml(string)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.SetDescriptionHtml instead.")]
    public static void SetDescriptionHtml(string descriptionHtml)
    {
        NewAllureApi.SetDescriptionHtml(descriptionHtml);
    }

    /// <summary>
    /// Adds new labels to the test's list of labels.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddLabels"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddLabels instead.")]
    public static void AddLabels(params Label[] labels)
    {
        NewAllureApi.AddLabels(labels);
    }

    /// <summary>
    /// Adds a new label to the current test result.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddLabel(string, string)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddLabel instead.")]
    public static void AddLabel(string name, string value)
    {
        NewAllureApi.AddLabel(name, value);
    }

    /// <summary>
    /// Adds a label to the current test result.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddLabel(Label)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddLabel instead.")]
    public static void AddLabel(Label label)
    {
        NewAllureApi.AddLabel(label);
    }

    /// <summary>
    /// Sets the current test's severity.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.SetSeverity"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.SetSeverity instead.")]
    public static void SetSeverity(SeverityLevel severity)
    {
        NewAllureApi.SetSeverity(severity switch
        {
            SeverityLevel.trivial => Severity.Trivial,
            SeverityLevel.minor => Severity.Minor,
            SeverityLevel.normal => Severity.Normal,
            SeverityLevel.critical => Severity.Critical,
            SeverityLevel.blocker => Severity.Blocker,
            var value => (Severity)value,
        });
    }

    /// <summary>
    /// Sets the current test's owner.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.SetOwner"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.SetOwner instead.")]
    public static void SetOwner(string owner)
    {
        NewAllureApi.SetOwner(owner);
    }

    /// <summary>
    /// Sets the current test's ID.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.SetAllureId"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.SetAllureId instead.")]
    public static void SetAllureId(int allureId)
    {
        NewAllureApi.SetAllureId(allureId);
    }

    /// <summary>
    /// Adds tags to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddTags"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddTags instead.")]
    public static void AddTags(params string[] tags)
    {
        NewAllureApi.AddTags(tags);
    }

    /// <summary>
    /// Adds an additional parent suite to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddParentSuite"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddParentSuite instead.")]
    public static void AddParentSuite(string parentSuite)
    {
        NewAllureApi.AddParentSuite(parentSuite);
    }

    /// <summary>
    /// Adds an additional suite to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddSuite"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddSuite instead.")]
    public static void AddSuite(string suite)
    {
        NewAllureApi.AddSuite(suite);
    }

    /// <summary>
    /// Adds an additional sub-suite to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddSubSuite"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddSubSuite instead.")]
    public static void AddSubSuite(string subSuite)
    {
        NewAllureApi.AddSubSuite(subSuite);
    }

    /// <summary>
    /// Adds an additional epic to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddEpic"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddEpic instead.")]
    public static void AddEpic(string epic)
    {
        NewAllureApi.AddEpic(epic);
    }

    /// <summary>
    /// Adds an additional feature to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddFeature"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddFeature instead.")]
    public static void AddFeature(string feature)
    {
        NewAllureApi.AddFeature(feature);
    }

    /// <summary>
    /// Adds an additional story to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddStory"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddStory instead.")]
    public static void AddStory(string story)
    {
        NewAllureApi.AddStory(story);
    }

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddLink(string)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddLink instead.")]
    public static void AddLink(string url)
    {
        NewAllureApi.AddLink(url);
    }

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddLink(string, string)"/>.
    /// Please, note, that the new function has a different parameter order: 'url, name' instead of 'name, url'.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddLink instead. Make sure to update the argument order from 'name, url' to 'url, name'")]
    public static void AddLink(string name, string url)
    {
        NewAllureApi.AddLink(url, name);
    }

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddLink(string, string, string)"/>.
    /// Please, note, that the new function has a different parameter order: 'name, url, type' instead of 'name, type, url'.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddLink instead. Make sure to update the argument order from 'name, type, url' to 'url, name, type'")]
    public static void AddLink(string name, string type, string url)
    {
        NewAllureApi.AddLink(url, name, type);
    }

    /// <summary>
    /// Adds new links to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddLinks"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddLinks instead.")]
    public static void AddLinks(params Link[] links)
    {
        NewAllureApi.AddLinks(links);
    }

    /// <summary>
    /// Adds a new issue link to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddIssue(string)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddIssue instead.")]
    public static void AddIssue(string url)
    {
        NewAllureApi.AddIssue(url);
    }

    /// <summary>
    /// Adds a new issue link to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddIssue(string, string)"/>.
    /// Please, note, that the new function has a different parameter order: 'url, name' instead of 'name, url'.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddIssue instead. Make sure to update the argument order from 'name, url' to 'url, name'")]
    public static void AddIssue(string name, string url)
    {
        NewAllureApi.AddIssue(url, name);
    }

    /// <summary>
    /// Adds a new TMS item link to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddTmsItem(string)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddTmsItem instead.")]
    public static void AddTmsItem(string url)
    {
        NewAllureApi.AddTmsItem(url);
    }

    /// <summary>
    /// Adds a new TMS item link to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddTmsItem(string, string)"/>.
    /// Please, note, that the new function has a different parameter order: 'url, name' instead of 'name, url'.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddTmsItem instead. Make sure to update the argument order from 'name, url' to 'url, name'")]
    public static void AddTmsItem(string name, string url)
    {
        NewAllureApi.AddTmsItem(url, name);
    }

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.Step(string)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.Step instead.")]
    public static void Step(string name)
    {
        NewAllureApi.Step(name);
    }

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.Step(string, Action)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.Step instead.")]
    public static void Step(string name, Action action)
    {
        NewAllureApi.Step(name, action);
    }

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.Step{TResult}(string, Func{TResult})"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.Step instead.")]
    public static T Step<T>(string name, Func<T> function) =>
        NewAllureApi.Step(name, function);

    /// <summary>
    /// Executes the asynchronous action and reports the result as a new step
    /// of the current fixture, test or step.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.StepAsync(string, Func{Task})"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.StepAsync instead.")]
    public static Task Step(string name, Func<Task> action) =>
        NewAllureApi.StepAsync(name, action);

    /// <summary>
    /// Executes the asynchronous function and reports the result as a new step
    /// of the current fixture, test or step.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.StepAsync{TResult}(string, Func{Task{TResult}})"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.StepAsync instead.")]
    public static Task<T> Step<T>(string name, Func<Task<T>> function) =>
        NewAllureApi.StepAsync(name, function);

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddAttachmentFromFile(string, string, string)"/>.
    /// Please, note, that the new function has a different parameter order: 'path, name, type' instead of 'name, type, path'.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddAttachmentFromFile instead. Make sure to update the argument order from 'name, type, path' to 'path, name, type'")]
    public static void AddAttachment(
        string name,
        string? type,
        string path
    )
    {
        NewAllureApi.AddAttachmentFromFile(path: path, name: name, mediaType: type);
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddAttachment(string, ReadOnlyMemory{byte}, string?, string)"/>.
    /// Please, note, that the new function has a different parameter order: 'name, content, type, extension' instead of 'name, type, content, extension'.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddAttachment instead. Make sure to update the argument order from 'name, type, content, extension' to 'name, content, type, extension'")]
    public static void AddAttachment(
        string name,
        string? type,
        byte[] content,
        string fileExtension = ""
    )
    {
        NewAllureApi.AddAttachment(
            name: name,
            content: content,
            mediaType: type,
            fileExtension: fileExtension
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step. Derives the media type
    /// from the file extension.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddAttachmentFromFile(string, string)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddAttachmentFromFile instead.")]
    public static void AddAttachment(
        string path,
        string? name = null
    )
    {
        NewAllureApi.AddAttachmentFromFile(path, name);
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddGlobalAttachmentFromFile(string, string, string)"/>.
    /// Please, note, that the new function has a different parameter order: 'path, name, type' instead of 'name, type, path'.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddGlobalAttachmentFromFile instead. Make sure to update the argument order from 'name, type, path' to 'path, name, type'")]
    public static void AddGlobalAttachment(
        string name,
        string? type,
        string path
    )
    {
        NewAllureApi.AddGlobalAttachmentFromFile(
            path: path,
            name: name,
            mediaType: type
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddGlobalAttachment(string, ReadOnlyMemory{byte}, string?, string)"/>.
    /// Please, note, that the new function has a different parameter order: 'name, content, type, extension' instead of 'name, type, content, extension'.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddGlobalAttachment instead. Make sure to update the argument order from 'name, type, content, extension' to 'name, content, type, extension'")]
    public static void AddGlobalAttachment(
        string name,
        string? type,
        byte[] content,
        string fileExtension = ""
    )
    {
        NewAllureApi.AddGlobalAttachment(
            name: name,
            content: content,
            mediaType: type,
            fileExtension: fileExtension
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// Derives the media type from the file extension.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddGlobalAttachmentFromFile(string, string)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddGlobalAttachmentFromFile instead.")]
    public static void AddGlobalAttachment(
        string path,
        string? name = null
    )
    {
        NewAllureApi.AddGlobalAttachmentFromFile(path, name);
    }

    /// <summary>
    /// Adds a global error not tied to the current fixture, test, or step.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddGlobalError(Exception)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddGlobalError instead.")]
    public static void AddGlobalError(Exception error)
    {
        NewAllureApi.AddGlobalError(error);
    }

    /// <summary>
    /// Adds a global error not tied to the current fixture, test, or step.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddGlobalError(string)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddGlobalError instead.")]
    public static void AddGlobalError(string message)
    {
        NewAllureApi.AddGlobalError(message);
    }

    /// <summary>
    /// Adds a global error not tied to the current fixture, test, or step.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddGlobalError(StatusDetails)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddGlobalError instead.")]
    public static void AddGlobalError(StatusDetails statusDetails)
    {
        NewAllureApi.AddGlobalError(statusDetails);
    }

    /// <summary>
    /// Attaches screen diff images to the current fixture, test, or step.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddScreenDiffFromFiles"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddScreenDiffFromFiles instead.")]
    public static void AddScreenDiff(
        string expectedPngPath,
        string actualPngPath,
        string diffPngPath
    )
    {
        NewAllureApi.AddScreenDiffFromFiles(expectedPngPath, actualPngPath, diffPngPath);
    }

    /// <summary>
    /// Attaches screen diff images to the current fixture, test, or step.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddScreenDiff(ReadOnlyMemory{byte}, ReadOnlyMemory{byte}, ReadOnlyMemory{byte})"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddScreenDiff instead.")]
    public static void AddScreenDiff(
        byte[] expectedPng,
        byte[] actualPng,
        byte[] diffPng
    )
    {
        NewAllureApi.AddScreenDiff(expectedPng, actualPng, diffPng);
    }

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddTestParameterFromObject(string, object?)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddTestParameterFromObject instead.")]
    public static void AddTestParameter(string name, object? value)
    {
        NewAllureApi.AddTestParameterFromObject(name, value);
    }

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddTestParameterFromObject(string, object?, ParameterMode)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddTestParameterFromObject instead.")]
    public static void AddTestParameter(string name, object? value, ParameterMode mode)
    {
        NewAllureApi.AddTestParameterFromObject(name, value, mode);
    }

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddTestParameterFromObject(string, object?, bool)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddTestParameterFromObject instead.")]
    public static void AddTestParameter(string name, object? value, bool excluded)
    {
        NewAllureApi.AddTestParameterFromObject(name, value, excluded);
    }

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddTestParameterFromObject(string, object?, ParameterMode, bool)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddTestParameterFromObject instead.")]
    public static void AddTestParameter(
        string name,
        object? value,
        ParameterMode mode,
        bool excluded
    )
    {
        NewAllureApi.AddTestParameterFromObject(name, value, mode, excluded);
    }

    /// <summary>
    /// Adds a new parameter to the current test. Use this overload if you
    /// want to manually control how the parameter's value should be displayed
    /// in the report.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.AddTestParameter(Parameter)"/>.
    /// </remarks>
    [Obsolete("Use Allure.AllureApi.AddTestParameter instead.")]
    public static void AddTestParameter(Parameter parameter)
    {
        NewAllureApi.AddTestParameter(parameter);
    }
}
