using Allure.Net.Commons;

namespace Allure.TestingPlatform.Properties;

public sealed class AllureStatusProperty<TObject>(Status status) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public Status Value { get; } = status;

    public bool OverwriteDefault { get; init; } = true;

    public void Apply(IAllureInfrastructure _, TObject obj)
    {
        if (!this.OverwriteDefault || obj.status is Status.none)
        {
            obj.status = this.Value;
        }
    }
}