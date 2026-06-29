using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureDescriptionHtmlProperty<TObject>(string descriptionHtml) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public string DescriptionHtml { get; } = descriptionHtml;

    public bool Append { get; init; } = false;

    public void Apply(ReadyAllureTestingPlatformRuntime _, TObject obj)
    {
        if (this.Append && obj.descriptionHtml is { Length: > 0 })
        {
            obj.descriptionHtml += $"{this.DescriptionHtml}";
        }
        else
        {
            obj.descriptionHtml = this.DescriptionHtml;
        }
    }
}