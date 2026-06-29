using System;
using System.Linq;
using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using Allure.Net.Commons.Functions;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureDefaultSuitesProperty(
    string? parentSuite,
    string? suite,
    string? subSuite
) :
    IAllureProperty<TestResult>
{
    public string? ParentSuite { get; } = parentSuite;
    public string? Suite { get; } = suite;
    public string? SubSuite { get; } = subSuite;

    public AllureDefaultSuitesProperty(Type testClass) : this(
        testClass.Assembly.GetName().Name,
        testClass.Namespace,
        ResolveSubSuite(testClass))
    {
    }

    public void Apply(LiveAllureTestingPlatformRuntime _, TestResult target)
    {
        ModelFunctions.EnsureSuites(target, this.ParentSuite, this.Suite, this.SubSuite);
    }

    static string? ResolveSubSuite(Type testClass) =>
        AllureApiAttribute
            .GetTypeAttributes(testClass)
            .OfType<AllureNameAttribute>()
            .LastOrDefault()
            ?.Name
        ?? testClass.Name;
}