using Allure.Sdk.Configuration;

namespace Allure.Sdk.Runtime;

/// <summary>
/// Exposes an Allure runtime with a strongly typed configuration.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
public interface IAllureRuntime<out TConfiguration> : IAllureRuntimeBase
    where TConfiguration : AllureConfiguration
{
    /// <summary>
    /// Gets the strongly typed runtime configuration.
    /// </summary>
    new TConfiguration Configuration { get; }
}
