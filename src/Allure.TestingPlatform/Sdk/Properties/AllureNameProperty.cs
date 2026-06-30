using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets the display name of an Allure test, step, or fixture.
/// </summary>
public sealed class AllureNameProperty<TModel>(string name) : IAllureProperty<TModel>
    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets the name to set.
    /// </summary>
    public string Name { get; } = name;

    /// <inheritdoc />
    public void Apply(LiveAllureTestingPlatformRuntime _, TModel target)
    {
        target.name = this.Name;
    }
}
