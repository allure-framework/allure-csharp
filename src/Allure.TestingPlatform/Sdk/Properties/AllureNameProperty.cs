using Allure.Net.Commons;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureNameProperty<TObject>(string name) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public string Name { get; } = name;

    public void Apply(IAllureRuntime _, TObject obj)
    {
        obj.name = this.Name;
    }
}