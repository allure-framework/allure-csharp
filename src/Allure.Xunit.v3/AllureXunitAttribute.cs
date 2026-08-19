using System;
using System.Collections.Generic;
using Xunit.Runner.Common;
using Xunit.v3;
using System.Reflection;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.Xunit.Internal.Registration;

namespace Allure.Xunit;

/// <summary>
/// Enables Allure.Xunit.v3 for an xUnit.net v3 test assembly.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class AllureXunitAttribute() :
    Attribute,
    IBeforeAfterTestAttribute,
    IRegisterRunnerReporterAttribute,
    ITraitAttribute
{
    /// <inheritdoc />
    public Type RunnerReporterType => typeof(AllureRunnerReporter);

    /// <inheritdoc />
    public void After(MethodInfo methodUnderTest, IXunitTest test) { }

    /// <inheritdoc />
    public void Before(MethodInfo methodUnderTest, IXunitTest test)
    {
        if (!AllureXunitRegistration.IsEnabled)
        {
            return;
        }

        AllureXunitRegistration.Current.RuntimeReference.Value.XunitMessageHandler.HandleBeforeTest(
            methodUnderTest,
            test,
            test.TestMethodArguments
        );
    }

    /// <inheritdoc />
    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits() =>
        AllureXunitRegistration.IsEnabled
            ? [new(TestNodeMetadataCorrelationStrategy.MetadataKey, TestNodeMetadataCorrelationStrategy.CreateCorrelationUid())]
            : [];
}
