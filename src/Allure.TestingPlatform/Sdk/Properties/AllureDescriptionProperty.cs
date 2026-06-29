using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureDescriptionProperty<TModel>(string description) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    public string Description { get; } = description;

    public bool Append { get; init; } = false;

    public void Apply(LiveAllureTestingPlatformRuntime _, TModel target)
    {
        if (this.Append && target.description is { Length: > 0 })
        {
            target.description += $"\n\n{this.Description}";
        }
        else
        {
            target.description = this.Description;
        }
    }
}