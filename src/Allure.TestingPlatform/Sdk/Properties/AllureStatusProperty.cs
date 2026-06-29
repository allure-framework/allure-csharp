using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureStatusProperty<TModel>(Status status) : IAllureProperty<TModel>
    where TModel : ExecutableItem
{
    public Status Status { get; } = status;

    public bool OnlyIfUnset { get; init; } = false;

    public void Apply(LiveAllureTestingPlatformRuntime _, TModel target)
    {
        if (!this.OnlyIfUnset || target.status is Status.none)
        {
            target.status = this.Status;
        }
    }
}