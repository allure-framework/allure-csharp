using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets status details on an Allure test, step, or fixture.
/// </summary>
public sealed class AllureStatusDetailsProperty<TModel>(StatusDetails statusDetails) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets the status details to set.
    /// </summary>
    public StatusDetails StatusDetails { get; } = statusDetails;

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TModel target)
    {
        target.StatusDetails = this.StatusDetails;
    }
}
