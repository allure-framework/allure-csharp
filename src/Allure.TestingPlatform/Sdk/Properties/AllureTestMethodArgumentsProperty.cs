using System.Collections.Generic;
using System.Reflection;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Adds Allure test parameters from test method arguments.
/// </summary>
public sealed class AllureTestMethodArgumentsProperty<TModel>(
    MethodInfo testMethod,
    IEnumerable<object> arguments
) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets the test method that declares the parameters.
    /// </summary>
    public MethodInfo TestMethod { get; } = testMethod;

    /// <summary>
    /// Gets the argument values.
    /// </summary>
    public List<object> Arguments { get; } = [..arguments];

    /// <inheritdoc />
    public void Apply(LiveAllureTestingPlatformRuntime allure, TModel target)
    {
        target.parameters.AddRange(
            ModelFunctions.CreateParameters(
                this.TestMethod.GetParameters(),
                this.Arguments,
                allure.TypeFormatters
            )
        );
    }
}
