using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureStatusDetailsProperty<TObject>(StatusDetails statusDetails) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public StatusDetails Value { get; } = statusDetails;

    public void Apply(LiveAllureTestingPlatformRuntime _, TObject obj)
    {
        obj.statusDetails = this.Value;
    }
}