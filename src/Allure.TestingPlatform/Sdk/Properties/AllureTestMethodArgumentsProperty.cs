using System.Collections.Generic;
using System.Reflection;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureTestMethodArgumentsProperty<TObject>(MethodInfo testMethod, IEnumerable<object> arguments) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public MethodInfo TestMethod { get; } = testMethod;

    public List<object> Arguments { get; } = [..arguments];

    public void Apply(IAllureInfrastructure allure, TObject obj)
    {
        obj.parameters.AddRange(
            ModelFunctions.CreateParameters(
                this.TestMethod.GetParameters(),
                this.Arguments,
                allure.TypeFormatters
            )
        );
    }
}