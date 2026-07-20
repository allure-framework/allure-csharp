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
        AllureFrontend.Runtime.TestApi.Async.AddLabels(labels, default);

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
        AllureFrontend.Runtime.TestApi.Async.AddLabels(labels, cancellationToken);

    /// <summary>
    /// Adds a new label to the current test result.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the label to add.</param>
    /// <param name="value">The value of the label to add.</param>
    public static Task AddLabelAsync(string name, string value) =>
        AllureFrontend.Runtime.TestApi.Async.AddLabel(
            new() { Name = name, Value = value },
            default
        );

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
        AllureFrontend.Runtime.TestApi.Async.AddLabel(
            new() { Name = name, Value = value },
            cancellationToken
        );

    /// <summary>
    /// Adds a label to the current test result.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="label">The new label of the test.</param>
    public static Task AddLabelAsync(Label label) =>
        AllureFrontend.Runtime.TestApi.Async.AddLabel(label, default);

    /// <summary>
    /// Adds a label to the current test result.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="label">The new label of the test.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddLabelAsync(Label label, CancellationToken cancellationToken) =>
        AllureFrontend.Runtime.TestApi.Async.AddLabel(label, cancellationToken);

    /// <summary>
    /// Sets the current test's severity.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="severity">The new severity level of the test.</param>
    public static Task SetSeverityAsync(Severity severity) =>
        AllureFrontend.Runtime.TestApi.Async.SetLabel(
            LabelName.Severity,
            severity.ToLabelValue(),
            default
        );

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
        AllureFrontend.Runtime.TestApi.Async.SetLabel(
            LabelName.Severity,
            severity.ToLabelValue(),
            cancellationToken
        );

    /// <summary>
    /// Sets the current test's owner.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="owner">The new owner of the test.</param>
    public static Task SetOwnerAsync(string owner) =>
        AllureFrontend.Runtime.TestApi.Async.SetLabel(
            LabelName.Owner,
            owner,
            default
        );

    /// <summary>
    /// Sets the current test's owner.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="owner">The new owner of the test.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task SetOwnerAsync(string owner, CancellationToken cancellationToken) =>
        AllureFrontend.Runtime.TestApi.Async.SetLabel(
            LabelName.Owner,
            owner,
            cancellationToken
        );

    /// <summary>
    /// Sets the current test's ID.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="allureId">The new ID of the test case.</param>
    public static Task SetAllureIdAsync(int allureId) =>
        AllureFrontend.Runtime.TestApi.Async.SetLabel(
            LabelName.AllureId,
            Convert.ToString(allureId, CultureInfo.InvariantCulture),
            default
        );

    /// <summary>
    /// Sets the current test's ID.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="allureId">The new ID of the test case.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task SetAllureIdAsync(int allureId, CancellationToken cancellationToken) =>
        AllureFrontend.Runtime.TestApi.Async.SetLabel(
            LabelName.AllureId,
            Convert.ToString(allureId, CultureInfo.InvariantCulture),
            cancellationToken
        );

    /// <summary>
    /// Adds tags to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="tags">The new tags.</param>
    public static Task AddTagsAsync(params IEnumerable<string> tags) =>
        AllureFrontend.Runtime.TestApi.Async.AddLabels(
            tags.Select(Label.Tag),
            default
        );

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
        AllureFrontend.Runtime.TestApi.Async.AddLabels(
            tags.Select(Label.Tag),
            cancellationToken
        );

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
