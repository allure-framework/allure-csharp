using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureStatusProperty<TObject>(Status status) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public Status Value { get; } = status;

    public bool OverwriteDefaultOnly { get; init; } = false;

    public void Apply(ReadyAllureTestingPlatform _, TObject obj)
    {
        if (!this.OverwriteDefaultOnly || obj.status is Status.none)
        {
            obj.status = this.Value;
        }
    }
}