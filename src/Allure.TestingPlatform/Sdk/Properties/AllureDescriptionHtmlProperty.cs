using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureDescriptionHtmlProperty<TModel>(string descriptionHtml) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    public string DescriptionHtml { get; } = descriptionHtml;

    public bool Append { get; init; } = false;

    public void Apply(LiveAllureTestingPlatformRuntime _, TModel target)
    {
        if (this.Append && target.descriptionHtml is { Length: > 0 })
        {
            target.descriptionHtml += $"{this.DescriptionHtml}";
        }
        else
        {
            target.descriptionHtml = this.DescriptionHtml;
        }
    }
}