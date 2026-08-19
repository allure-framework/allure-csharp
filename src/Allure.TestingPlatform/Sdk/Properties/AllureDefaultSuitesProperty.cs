using System;
using System.Linq;
using Allure.Abstractions;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Internal.Functions;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets default suite labels on an Allure test result.
/// The labels are applied only when the result has no <c>parentSuite</c>, <c>suite</c>, or
/// <c>subSuite</c> label.
/// </summary>
/// <param name="parentSuite">The default parent-suite name.</param>
/// <param name="suite">The default suite name.</param>
/// <param name="subSuite">The default sub-suite name.</param>
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
    /// <param name="testClass">The test class from which suite names are derived.</param>
    public AllureDefaultSuitesProperty(Type testClass) : this(
        testClass.Assembly.GetName().Name,
        testClass.Namespace,
        ResolveSubSuite(testClass))
    {
    }

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TestResult target)
    {
        target.RememberDefaultSuites(this.ParentSuite, this.Suite, this.SubSuite);
    }

    static string? ResolveSubSuite(Type testClass) =>
        AllureApiAttribute
            .GetTypeAttributes(testClass)
            .OfType<AllureNameAttribute>()
            .LastOrDefault()
            ?.Name
        ?? testClass.Name;
}
