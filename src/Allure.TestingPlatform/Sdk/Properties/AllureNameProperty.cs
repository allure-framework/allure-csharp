using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureNameProperty<TModel>(string name) : IAllureProperty<TModel>
    where TModel : ExecutableItem
{
    public string Name { get; } = name;

    public void Apply(LiveAllureTestingPlatformRuntime _, TModel target)
    {
        target.name = this.Name;
    }
}