using System;

namespace Allure.Sdk.Configuration;

/// <summary>
/// Loads configuration by invoking a delegate.
/// </summary>
/// <typeparam name="TConfiguration">The configuration type.</typeparam>
/// <param name="name">A human-readable name for the source.</param>
/// <param name="factory">
/// The delegate that loads the configuration and identifies its assigned properties.
/// </param>
public sealed class DelegateConfigurationSource<TConfiguration>(
    string name,
    Func<TrackedConfiguration<TConfiguration>> factory
) :
    IAllureConfigurationSource<TConfiguration>

    where TConfiguration : AllureConfiguration
{
    /// <inheritdoc/>
    public string Name => name;

    /// <inheritdoc/>
    public bool CanLoad => true;

    /// <summary>
    /// Creates a source that loads the configuration by invoking a delegate.
    /// </summary>
    /// <param name="name">A human-readable name for the source.</param>
    /// <param name="factory">
    /// The delegate that creates the configuration.
    /// All readable, non-indexed public properties of the created configuration are
    /// considered assigned.
    /// </param>
    public DelegateConfigurationSource(
        string name,
        Func<TConfiguration> factory
    ) : this(
        name,
        () => TrackedConfiguration.WithAllPropertiesSet(
            name,
            factory()
        )
    )
    {
    }

    /// <inheritdoc/>
    public TrackedConfiguration<TConfiguration> LoadConfiguration() => factory();
}

/// <summary>
/// Creates delegate-backed configuration sources.
/// </summary>
public static class DelegateConfigurationSource
{
    /// <summary>
    /// Creates a configuration source that invokes the specified factory.
    /// </summary>
    /// <typeparam name="TConfiguration">The configuration type.</typeparam>
    /// <param name="name">A human-readable name for the source.</param>
    /// <param name="configurationFactory">
    /// The delegate that loads the configuration and identifies its assigned properties.
    /// </param>
    /// <returns>The configuration source.</returns>
    public static DelegateConfigurationSource<TConfiguration> Create<TConfiguration>(
        string name,
        Func<TrackedConfiguration<TConfiguration>> configurationFactory
    )
        where TConfiguration : AllureConfiguration
    =>
        new(name, configurationFactory);

    /// <summary>
    /// Creates a configuration source that invokes the specified factory.
    /// </summary>
    /// <typeparam name="TConfiguration">The configuration type.</typeparam>
    /// <param name="name">A human-readable name for the source.</param>
    /// <param name="configurationFactory">
    /// The delegate that creates the configuration.
    /// All readable, non-indexed public properties of the created configuration are
    /// considered assigned.
    /// </param>
    /// <returns>The configuration source.</returns>
    public static DelegateConfigurationSource<TConfiguration> Create<TConfiguration>(
        string name,
        Func<TConfiguration> configurationFactory
    )
        where TConfiguration : AllureConfiguration
    =>
        new(name, configurationFactory);
}
