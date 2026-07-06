using System;
using System.Collections.Generic;
using Xunit.Runner.Common;
using Xunit.v3;
using System.Reflection;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.Xunit.Internal;

namespace Allure.Xunit;

[AttributeUsage(AttributeTargets.Assembly)]
public class AllureXunitAttribute() :
    Attribute,
    IBeforeAfterTestAttribute,
    IRegisterRunnerReporterAttribute,
    ITraitAttribute
{
    public Type RunnerReporterType => typeof(AllureRunnerReporter);

    public void After(MethodInfo methodUnderTest, IXunitTest test) { }

    public void Before(MethodInfo methodUnderTest, IXunitTest test) =>
        AllureRunnerReporter.CurrentMessageHandler?.HandleBeforeTest(
            methodUnderTest,
            test,
            test.TestMethodArguments
        );

    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits() =>
        AllureTestingPlatformServices.IsAllureAlive
            ? [new(TestNodeMetadataCorrelationStrategy.MetadataKey, TestNodeMetadataCorrelationStrategy.CreateCorrelationUid())]
            : [];
}
