using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureStatusDetailsProperty<TModel>(StatusDetails statusDetails) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    public StatusDetails StatusDetails { get; } = statusDetails;

    public void Apply(LiveAllureTestingPlatformRuntime _, TModel target)
    {
        target.statusDetails = this.StatusDetails;
    }
}