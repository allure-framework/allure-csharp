using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureNameProperty<TObject>(string name) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public string Name { get; } = name;

    public void Apply(ReadyAllureTestingPlatform _, TObject obj)
    {
        obj.name = this.Name;
    }
}