using System;
using System.Linq;
using Allure.Model;
using Allure.Runtime;
using System.Collections.Generic;
using System.Globalization;
using Allure.Internal;

namespace Allure;

public static partial class AllureApi
{
    /// <summary>
    /// Adds new labels to the test's list of labels.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="labels">The labels to add.</param>
    public static void AddLabels(params IEnumerable<Label> labels) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddLabels(labels);

    /// <summary>
    /// Adds a new label to the current test result.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the label to add.</param>
    /// <param name="value">The value of the label to add.</param>
    public static void AddLabel(string name, string value) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddLabel(
            new() { Name = name, Value = value }
        );

    /// <summary>
    /// Adds a label to the current test result.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="label">The new label of the test.</param>
    public static void AddLabel(Label label) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddLabel(label);

    /// <summary>
    /// Sets the current test's severity.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="severity">The new severity level of the test.</param>
    public static void SetSeverity(Severity severity) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.SetLabel(
            LabelName.Severity,
            severity.ToLabelValue()
        );

    /// <summary>
    /// Sets the current test's owner.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="owner">The new owner of the test.</param>
    public static void SetOwner(string owner) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.SetLabel(
            LabelName.Owner,
            owner
        );

    /// <summary>
    /// Sets the current test's ID.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="allureId">The new ID of the test case.</param>
    public static void SetAllureId(int allureId) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.SetLabel(
            LabelName.AllureId,
            Convert.ToString(allureId, CultureInfo.InvariantCulture)
        );

    /// <summary>
    /// Adds tags to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="tags">The new tags.</param>
    public static void AddTags(params IEnumerable<string> tags) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddLabels(
            tags.Select(Label.Tag)
        );

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
}
