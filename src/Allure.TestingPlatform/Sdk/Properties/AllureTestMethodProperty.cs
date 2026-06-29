using System;
using System.Collections.Generic;
using System.Reflection;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureTestMethodProperty(MethodInfo testMethod) :
    IAllureProperty<TestResult>
{
    public MethodInfo TestMethod { get; } = testMethod;

    public Type TestClass { get; init; } = testMethod.DeclaringType;

    public List<object?> Arguments { get; init; } = [];

    public AllureTestMethodUpdateTargets UpdateTargets { get; init; } = AllureTestMethodUpdateTargets.All;

    public void Apply(LiveAllureTestingPlatformRuntime allureRuntime, TestResult target)
    {
        if (this.ShouldSetFullName)
        {
            target.fullName = IdFunctions.CreateFullName(this.TestMethod);
        }

        if (this.ShouldSetTitlePath)
        {
            target.titlePath = IdFunctions.CreateTitlePath(this.TestMethod);
        }

        if (this.ShouldAddLabels)
        {
            target.labels.Add(Label.TestClass(this.TestClass.Name));
            target.labels.Add(Label.TestMethod(this.TestMethod.Name));
            target.labels.Add(Label.Package(this.TestClass.FullName));
        }

        if (this.ShouldAddParameters)
        {
            var parameters = ModelFunctions.CreateParameters(
                this.TestMethod.GetParameters(),
                this.Arguments,
                allureRuntime.TypeFormatters);
            target.parameters.AddRange(parameters);
        }

        if (this.ShouldApplyApiAttributes)
        {
            AllureApiAttribute.ApplyTypeAttributes(this.TestClass, target);
            AllureApiAttribute.ApplyMethodAttributes(this.TestMethod, target);
        }
    }

    bool ShouldSetFullName => this.ShouldUpdateTarget(AllureTestMethodUpdateTargets.FullName);

    bool ShouldAddParameters => this.ShouldUpdateTarget(AllureTestMethodUpdateTargets.Parameters);

    bool ShouldSetTitlePath => this.ShouldUpdateTarget(AllureTestMethodUpdateTargets.TitlePath);

    bool ShouldAddLabels => this.ShouldUpdateTarget(AllureTestMethodUpdateTargets.Labels);

    bool ShouldApplyApiAttributes => this.ShouldUpdateTarget(AllureTestMethodUpdateTargets.ApiAttributes);

    bool ShouldUpdateTarget(AllureTestMethodUpdateTargets target) => this.UpdateTargets.HasFlag(target);
}