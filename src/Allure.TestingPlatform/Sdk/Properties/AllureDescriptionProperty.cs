using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureDescriptionProperty<TObject>(string description) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public string Description { get; } = description;

    public bool Append { get; init; } = false;

    public void Apply(ReadyAllureTestingPlatformRuntime _, TObject obj)
    {
        if (this.Append && obj.description is { Length: > 0 })
        {
            obj.description += $"\n\n{this.Description}";
        }
        else
        {
            obj.description = this.Description;
        }
    }
}