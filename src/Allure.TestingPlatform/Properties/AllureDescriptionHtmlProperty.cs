using Allure.Net.Commons;

namespace Allure.TestingPlatform.Properties;

public sealed class AllureDescriptionHtmlProperty<TObject>(string descriptionHtml) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public string DescriptionHtml { get; } = descriptionHtml;

    public bool Append { get; init; } = false;

    public void Apply(IAllureInfrastructure _, TObject obj)
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