using Allure.Net.Commons;

namespace Allure.TestingPlatform.Properties;

public sealed class AllureNameProperty<TObject>(string name) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public string Name { get; } = name;

    public void Apply(IAllureInfrastructure _, TObject obj)
    {
        obj.name = this.Name;
    }
}