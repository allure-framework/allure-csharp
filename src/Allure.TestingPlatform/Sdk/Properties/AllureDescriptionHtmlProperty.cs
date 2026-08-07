using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets or appends the HTML description of an Allure test, step, or fixture.
/// </summary>
public sealed class AllureDescriptionHtmlProperty<TModel>(string descriptionHtml) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets the HTML description.
    /// </summary>
    public string DescriptionHtml { get; } = descriptionHtml;

    /// <summary>
    /// Gets or sets whether the HTML description should be appended to an existing description.
    /// </summary>
    public bool Append { get; init; } = false;

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TModel target)
    {
        if (this.Append && target.DescriptionHtml is { Length: > 0 })
        {
            target.DescriptionHtml += $"{this.DescriptionHtml}";
        }
        else
        {
            target.DescriptionHtml = this.DescriptionHtml;
        }
    }
}
