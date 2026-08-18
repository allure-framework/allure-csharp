using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Marks a Microsoft Testing Platform property as an Allure property.
/// </summary>
public interface IAllureProperty : IProperty;

/// <summary>
/// An implementation of this interface is expected to define how the property is
/// applied to the target model object.
/// </summary>
/// <typeparam name="TModel">The type of model object to which the property applies.</typeparam>
public interface IAllureProperty<TModel> : IAllureProperty
{
    /// <summary>
    /// Applies the property to the target model object.
    /// </summary>
    /// <param name="allureRuntime">The runtime whose services are available to the property.</param>
    /// <param name="target">The model object to update.</param>
    public abstract void Apply(
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime,
        TModel target
    );
}
