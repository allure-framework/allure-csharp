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
public interface IAllureProperty<TModel> : IAllureProperty
{
    /// <summary>
    /// Applies the property to the target model object.
    /// </summary>
    public abstract void Apply(LiveAllureTestingPlatformRuntime allureRuntime, TModel target);
}
