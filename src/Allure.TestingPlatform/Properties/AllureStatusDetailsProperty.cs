using Allure.Net.Commons;

namespace Allure.TestingPlatform.Properties;

public sealed class AllureStatusDetailsProperty<TObject>(StatusDetails statusDetails) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public StatusDetails Value { get; } = statusDetails;

    public void Apply(IAllureInfrastructure _, TObject obj)
    {
        obj.statusDetails = this.Value;
    }
}