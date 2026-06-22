using Allure.Net.Commons;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureDescriptionProperty<TObject>(string description) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public string Description { get; } = description;

    public bool Append { get; init; } = false;

    public void Apply(IAllureRuntime _, TObject obj)
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