using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureNameProperty<TObject>(string name) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public string Name { get; } = name;

    public void Apply(LiveAllureTestingPlatformRuntime _, TObject target)
    {
        target.name = this.Name;
    }
}