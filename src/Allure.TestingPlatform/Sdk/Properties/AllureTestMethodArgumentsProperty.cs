using System.Collections.Generic;
using System.Reflection;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureTestMethodArgumentsProperty<TModel>(
    MethodInfo testMethod,
    IEnumerable<object> arguments
) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    public MethodInfo TestMethod { get; } = testMethod;

    public List<object> Arguments { get; } = [..arguments];

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