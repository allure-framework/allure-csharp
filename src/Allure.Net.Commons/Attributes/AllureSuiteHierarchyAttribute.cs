using System;
using Allure.Net.Commons.Sdk;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies the whole suite hierarchy at once.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = false, Inherited = true)]
public class AllureSuiteHierarchyAttribute : AllureApiAttribute
{
    /// <summary>
    /// The value of the <c>parentSuite</c> label.
    /// </summary>
    public string? ParentSuite { get; init; }

    /// <summary>
    /// The value of the <c>suite</c> label.
    /// </summary>
    public string? Suite { get; init; }

    /// <summary>
    /// The value of the <c>subSuite</c> label.
    /// </summary>
    public string? SubSuite { get; init; }

    /// <summary>
    /// A shorthand for <see cref="AllureParentSuiteAttribute"/>,
    /// <see cref="AllureSuiteAttribute"/>, and <see cref="AllureSubSuiteAttribute"/>.
    /// </summary>
    public AllureSuiteHierarchyAttribute(string parentSuite, string suite, string subSuite)
    {
        this.ParentSuite = parentSuite;
        this.Suite = suite;
        this.SubSuite = subSuite;
    }

    /// <summary>
    /// A shorthand for <see cref="AllureParentSuiteAttribute"/> and
    /// <see cref="AllureSuiteAttribute"/>.
    /// </summary>
    public AllureSuiteHierarchyAttribute(string parentSuite, string suite)
    {
        this.ParentSuite = parentSuite;
        this.Suite = suite;
    }

    /// <summary>
    /// An alias for <see cref="AllureSuiteAttribute"/>.
    /// </summary>
    public AllureSuiteHierarchyAttribute(string suite)
    {
        this.Suite = suite;
    }

    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        var labels = testResult.labels;

        if (this.ParentSuite is not null)
        {
            labels.Add(Label.ParentSuite(this.ParentSuite));
        }

        if (this.Suite is not null)
        {
            labels.Add(Label.Suite(this.Suite));
        }

        if (this.SubSuite is not null)
        {
            labels.Add(Label.SubSuite(this.SubSuite));
        }
    }
}
