using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureParametersProperty<TObject>(IEnumerable<Parameter> parameters) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public List<Parameter> Parameters { get; } = [..parameters];

    public void Apply(ReadyAllureTestingPlatformRuntime _, TObject obj)
    {
        obj.parameters.AddRange(this.Parameters);
    }
}