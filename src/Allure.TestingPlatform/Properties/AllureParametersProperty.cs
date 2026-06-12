using System.Collections.Generic;
using Allure.Net.Commons;

namespace Allure.TestingPlatform.Properties;

public sealed class AllureParametersProperty<TObject>(IEnumerable<Parameter> parameters) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public List<Parameter> Parameters { get; } = [..parameters];

    public void Apply(IAllureInfrastructure _, TObject obj)
    {
        obj.parameters.AddRange(this.Parameters);
    }
}