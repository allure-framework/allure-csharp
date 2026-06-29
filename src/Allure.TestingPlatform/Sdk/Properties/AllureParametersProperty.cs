using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureParametersProperty<TModel>(IEnumerable<Parameter> parameters) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    public List<Parameter> Parameters { get; } = [..parameters];

    public void Apply(LiveAllureTestingPlatformRuntime _, TModel target)
    {
        target.parameters.AddRange(this.Parameters);
    }
}