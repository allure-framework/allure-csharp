using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureNameProperty<TObject>(string name) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public string Name { get; } = name;

    public void Apply(ReadyAllureTestingPlatformRuntime _, TObject obj)
    {
        obj.name = this.Name;
    }
}