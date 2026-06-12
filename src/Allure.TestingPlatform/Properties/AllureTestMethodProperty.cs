using System;
using System.Collections.Generic;
using System.Reflection;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.Net.Commons.Sdk;

namespace Allure.TestingPlatform.Properties;

[Flags]
public enum TestMethodUpdateTarget
{
    FullName = 0x01 << 0,
    TitlePath = 0x01 << 1,
    Labels = 0x01 << 2,
    Parameters = 0x01 << 3,
    ApiAttributes = 0x01 << 4,

    All = FullName | TitlePath | Labels | Parameters | ApiAttributes,
}

public sealed class AllureTestMethodProperty(MethodInfo testMethod) : IAllureProperty<TestResult>
{
    public MethodInfo TestMethod { get; } = testMethod;

    public Type TestClass { get; init; } = testMethod.DeclaringType;

    public List<object> Arguments { get; init; } = [];

    public TestMethodUpdateTarget UpdateTargets { get; init; } = TestMethodUpdateTarget.All;

    public void Apply(IAllureInfrastructure allure, TestResult obj)
    {
        if (this.ShouldSetFullName)
        {
            obj.fullName = IdFunctions.CreateFullName(this.TestMethod);
        }

        if (this.ShouldSetTitlePath)
        {
            obj.titlePath = IdFunctions.CreateTitlePath(this.TestMethod);
        }

        if (this.ShouldAddLabels)
        {
            obj.labels.Add(Label.TestClass(this.TestClass.Name));
            obj.labels.Add(Label.TestMethod(this.TestMethod.Name));
            obj.labels.Add(Label.Package(this.TestClass.FullName));
        }

        if (this.ShouldAddParameters)
        {
            var parameters = ModelFunctions.CreateParameters(
                this.TestMethod.GetParameters(),
                this.Arguments,
                allure.TypeFormatters);
            obj.parameters.AddRange(parameters);
        }

        if (this.ShouldApplyApiAttributes)
        {
            AllureApiAttribute.ApplyTypeAttributes(this.TestClass, obj);
            AllureApiAttribute.ApplyMethodAttributes(this.TestMethod, obj);
        }
    }

    bool ShouldSetFullName => this.ShouldUpdateTarget(TestMethodUpdateTarget.FullName);

    bool ShouldAddParameters => this.ShouldUpdateTarget(TestMethodUpdateTarget.Parameters);

    bool ShouldSetTitlePath => this.ShouldUpdateTarget(TestMethodUpdateTarget.TitlePath);

    bool ShouldAddLabels => this.ShouldUpdateTarget(TestMethodUpdateTarget.Labels);

    bool ShouldApplyApiAttributes => this.ShouldUpdateTarget(TestMethodUpdateTarget.ApiAttributes);

    bool ShouldUpdateTarget(TestMethodUpdateTarget target) => this.UpdateTargets.HasFlag(target);
}