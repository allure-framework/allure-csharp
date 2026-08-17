using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets the display name of an Allure test, step, or fixture.
/// </summary>
/// <typeparam name="TModel">The type of model object to update.</typeparam>
/// <param name="name">The display name to set.</param>
public sealed class AllureNameProperty<TModel>(string name) : IAllureProperty<TModel>
    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets the name to set.
    /// </summary>
    public string Name { get; } = name;

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TModel target)
    {
        target.Name = this.Name;
    }
}
