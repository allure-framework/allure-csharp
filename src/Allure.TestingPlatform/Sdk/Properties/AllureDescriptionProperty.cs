using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets or appends the Markdown description of an Allure step, test, or fixture.
/// </summary>
public sealed class AllureDescriptionProperty<TModel>(string description) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets the description Markdown text.
    /// </summary>
    public string Description { get; } = description;

    /// <summary>
    /// Gets or sets whether the description should be appended to an existing description.
    /// </summary>
    public bool Append { get; init; } = false;

    /// <inheritdoc />
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
