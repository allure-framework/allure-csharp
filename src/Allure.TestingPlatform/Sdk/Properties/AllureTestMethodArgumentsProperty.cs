using System.Collections.Generic;
using System.Reflection;
using Allure.Model;
using Allure.Sdk.Functions;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Adds Allure test parameters from test method arguments.
/// </summary>
/// <typeparam name="TModel">The type of model object to update.</typeparam>
/// <param name="testMethod">The test method that declares the parameters.</param>
/// <param name="arguments">The test method argument values.</param>
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
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allure, TModel target)
    {
        target.Parameters.AddRange(
            Parameters.Create(
                this.TestMethod.GetParameters(),
                this.Arguments,
                allure.ParameterSerializer
            )
        );
    }
}
