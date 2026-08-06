namespace Allure.Sdk.Configuration;

/// <summary>
/// Provides configuration values to an Allure runtime.
/// </summary>
/// <typeparam name="TConfiguration">The configuration type.</typeparam>
public interface IAllureConfigurationSource<TConfiguration>
    where TConfiguration : AllureConfiguration
{
    /// <summary>
    /// Gets a human-readable name for the source.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a value indicating whether the source can currently load configuration.
    /// </summary>
    bool CanLoad { get; }

    /// <summary>
    /// Loads configuration from the source.
    /// </summary>
    /// <returns>The loaded configuration.</returns>
    TrackedConfiguration<TConfiguration> LoadConfiguration();
}
