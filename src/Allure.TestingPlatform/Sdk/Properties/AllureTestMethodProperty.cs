using System;
using System.Collections.Generic;
using System.Reflection;
using Allure.Abstractions;
using Allure.Model;
using Allure.Sdk.Functions;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Updates an Allure test result from a reflected test method.
/// </summary>
/// <param name="testMethod">The reflected test method.</param>
public sealed class AllureTestMethodProperty(MethodInfo testMethod) :
    IAllureProperty<TestResult>
{
    /// <summary>
    /// Gets the reflected test method.
    /// </summary>
    public MethodInfo TestMethod { get; } = testMethod;

    /// <summary>
    /// Gets or sets the reflected test class.
    /// </summary>
    public Type TestClass { get; init; } = testMethod.DeclaringType;

    /// <summary>
    /// Gets or sets the test method argument values.
    /// </summary>
    public List<object?> Arguments { get; init; } = [];

    /// <summary>
    /// Gets or sets which test result fields are updated.
    /// </summary>
    public AllureTestMethodUpdateTargets UpdateTargets { get; init; } =
        AllureTestMethodUpdateTargets.All;

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime, TestResult target)
    {
        if (this.ShouldSetFullName)
        {
            target.FullName = ReflectionNames.ForMethod(this.TestMethod);
        }

        if (this.ShouldSetTitlePath)
        {
            target.TitlePath.Clear();
            target.TitlePath.AddRange([.. Titles.PathFor(this.TestMethod)]);
        }

        if (this.ShouldAddLabels)
        {
            target.Labels.Add(Label.TestClass(this.TestClass.Name));
            target.Labels.Add(Label.TestMethod(this.TestMethod.Name));
            target.Labels.Add(Label.Package(this.TestClass.FullName));
        }

        if (this.ShouldAddParameters)
        {
            var parameters = Parameters.Create(
                this.TestMethod.GetParameters(),
                this.Arguments,
                allureRuntime.ParameterSerializer
            );
            target.Parameters.AddRange(parameters);
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
