using System;
using System.Linq;
using Allure.Model;
using Allure.Runtime;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using Allure.Internal;

namespace Allure;

/// <summary>
/// A facade that provides the API for test authors to enhance the Allure
/// report.
/// </summary>
public static partial class AllureApi
{
    /// <summary>
    /// Adds new labels to the test's list of labels.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="labels">The labels to add.</param>
    public static Task AddLabelsAsync(params IEnumerable<Label> labels) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddLabelsAsync(labels, default)
            ?? Task.CompletedTask;

    /// <summary>
    /// Adds new labels to the test's list of labels.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="labels">The labels to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddLabelsAsync(
        IEnumerable<Label> labels,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddLabelsAsync(labels, cancellationToken)
            ?? Task.CompletedTask;

    /// <summary>
    /// Adds a new label to the current test result.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the label to add.</param>
    /// <param name="value">The value of the label to add.</param>
    public static Task AddLabelAsync(string name, string value) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddLabelAsync(
            new() { Name = name, Value = value },
            default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a new label to the current test result.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the label to add.</param>
    /// <param name="value">The value of the label to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddLabelAsync(
        string name,
        string value,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddLabelAsync(
            new() { Name = name, Value = value },
            cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a label to the current test result.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="label">The new label of the test.</param>
    public static Task AddLabelAsync(Label label) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddLabelAsync(label, default)
            ?? Task.CompletedTask;

    /// <summary>
    /// Adds a label to the current test result.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="label">The new label of the test.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddLabelAsync(Label label, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddLabelAsync(label, cancellationToken)
            ?? Task.CompletedTask;

    /// <summary>
    /// Sets the current test's severity.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="severity">The new severity level of the test.</param>
    public static Task SetSeverityAsync(Severity severity) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetLabelAsync(
            LabelName.Severity,
            severity.ToLabelValue(),
            default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Sets the current test's severity.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="severity">The new severity level of the test.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task SetSeverityAsync(
        Severity severity,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetLabelAsync(
            LabelName.Severity,
            severity.ToLabelValue(),
            cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Sets the current test's owner.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="owner">The new owner of the test.</param>
    public static Task SetOwnerAsync(string owner) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetLabelAsync(
            LabelName.Owner,
            owner,
            default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Sets the current test's owner.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="owner">The new owner of the test.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task SetOwnerAsync(string owner, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetLabelAsync(
            LabelName.Owner,
            owner,
            cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Sets the current test's ID.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="allureId">The new ID of the test case.</param>
    public static Task SetAllureIdAsync(int allureId) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetLabelAsync(
            LabelName.AllureId,
            Convert.ToString(allureId, CultureInfo.InvariantCulture),
            default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Sets the current test's ID.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="allureId">The new ID of the test case.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task SetAllureIdAsync(int allureId, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetLabelAsync(
            LabelName.AllureId,
            Convert.ToString(allureId, CultureInfo.InvariantCulture),
            cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds tags to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="tags">The new tags.</param>
    public static Task AddTagsAsync(params IEnumerable<string> tags) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddLabelsAsync(
            tags.Select(Label.Tag),
            default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds tags to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="tags">The new tags.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddTagsAsync(
        IEnumerable<string> tags,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddLabelsAsync(
            tags.Select(Label.Tag),
            cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds an additional parent suite to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="parentSuite">The parent suite to be added.</param>
    public static Task AddParentSuiteAsync(string parentSuite) =>
        AddLabelAsync(
            Label.ParentSuite(parentSuite)
        );

    /// <summary>
    /// Adds an additional parent suite to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="parentSuite">The parent suite to be added.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddParentSuiteAsync(
        string parentSuite,
        CancellationToken cancellationToken
    ) =>
        AddLabelAsync(
            Label.ParentSuite(parentSuite),
            cancellationToken
        );

    /// <summary>
    /// Adds an additional suite to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="suite">The suite to be added.</param>
    public static Task AddSuiteAsync(string suite) =>
        AddLabelAsync(
            Label.Suite(suite)
        );

    /// <summary>
    /// Adds an additional suite to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="suite">The suite to be added.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddSuiteAsync(string suite, CancellationToken cancellationToken) =>
        AddLabelAsync(
            Label.Suite(suite),
            cancellationToken
        );

    /// <summary>
    /// Adds an additional sub-suite to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="subSuite">The sub-suite to be added.</param>
    public static Task AddSubSuiteAsync(string subSuite) =>
        AddLabelAsync(
            Label.SubSuite(subSuite)
        );

    /// <summary>
    /// Adds an additional sub-suite to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="subSuite">The sub-suite to be added.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddSubSuiteAsync(
        string subSuite,
        CancellationToken cancellationToken
    ) =>
        AddLabelAsync(
            Label.SubSuite(subSuite),
            cancellationToken
        );

    /// <summary>
    /// Adds an additional epic to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="epic">The epic to be added.</param>
    public static Task AddEpicAsync(string epic) =>
        AddLabelAsync(
            Label.Epic(epic)
        );

    /// <summary>
    /// Adds an additional epic to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="epic">The epic to be added.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddEpicAsync(string epic, CancellationToken cancellationToken) =>
        AddLabelAsync(
            Label.Epic(epic),
            cancellationToken
        );

    /// <summary>
    /// Adds an additional feature to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="feature">The feature to be added.</param>
    public static Task AddFeatureAsync(string feature) =>
        AddLabelAsync(
            Label.Feature(feature)
        );

    /// <summary>
    /// Adds an additional feature to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="feature">The feature to be added.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddFeatureAsync(
        string feature,
        CancellationToken cancellationToken
    ) =>
        AddLabelAsync(
            Label.Feature(feature),
            cancellationToken
        );

    /// <summary>
    /// Adds an additional story to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="story">The story to be added.</param>
    public static Task AddStoryAsync(string story) =>
        AddLabelAsync(
            Label.Story(story)
        );


    /// <summary>
    /// Adds an additional story to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="story">The story to be added.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddStoryAsync(
        string story,
        CancellationToken cancellationToken
    ) =>
        AddLabelAsync(
            Label.Story(story),
            cancellationToken
        );
}
