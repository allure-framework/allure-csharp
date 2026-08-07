using System;
using System.Linq;
using Allure.Abstractions;
using Allure.Model;
using Allure.Sdk.Functions;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets default suite labels on an Allure test result.
/// The labels will be applied after the test stops, but only if
/// the test has neither parentSuite, nor suite, nor subSuite labels.
/// </summary>
public sealed class AllureDefaultSuitesProperty(
    string? parentSuite,
    string? suite,
    string? subSuite
) :
    IAllureProperty<TestResult>
{
    /// <summary>
    /// Gets the parent suite name.
    /// </summary>
    public string? ParentSuite { get; } = parentSuite;

    /// <summary>
    /// Gets the suite name.
    /// </summary>
    public string? Suite { get; } = suite;

    /// <summary>
    /// Gets the sub-suite name.
    /// </summary>
    public string? SubSuite { get; } = subSuite;

    /// <summary>
    /// Creates suite labels from the specified test class.
    /// </summary>
    public AllureDefaultSuitesProperty(Type testClass) : this(
        testClass.Assembly.GetName().Name,
        testClass.Namespace,
        ResolveSubSuite(testClass))
    {
    }

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TestResult target)
    {
        SuiteLabels.Ensure(target, this.ParentSuite, this.Suite, this.SubSuite);
    }

    static string? ResolveSubSuite(Type testClass) =>
        AllureApiAttribute
            .GetTypeAttributes(testClass)
            .OfType<AllureNameAttribute>()
            .LastOrDefault()
            ?.Name
        ?? testClass.Name;
}
