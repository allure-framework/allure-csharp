using System;
using System.Text.Json;
using Allure.Sdk.Configuration;
using Allure.Sdk.Serialization;

namespace Allure.Sdk.Registration;

/// <summary>
/// Provides convenient configuration-source registration methods.
/// </summary>
public static class AllureRegistrationContextExtensions
{
    /// <summary>
    /// Provides convenient configuration-source registration methods.
    /// </summary>
    /// <typeparam name="TConfiguration">A configuration type.</typeparam>
    /// <param name="context">A runtime registration context.</param>
    extension<TConfiguration>(
        IAllureRuntimeRegistrationContext<TConfiguration> context
    )
        where TConfiguration : AllureConfiguration, new()
    {
        /// <summary>
        /// Loads configuration from the specified source.
        /// </summary>
        /// <param name="sourceFactory">A function that creates the configuration source.</param>
        public void UseConfigurationSource(Func<IAllureConfigurationSource<TConfiguration>> sourceFactory) =>
            context.UseConfigurationSources(() => [sourceFactory()]);

        /// <summary>
        /// Loads configuration from the specified JSON file.
        /// </summary>
        public void UseConfigurationFile(string path) =>
            context.UseConfigurationSources(
                () => [new JsonFileConfigurationSource<TConfiguration>(path)]
            );

        /// <summary>
        /// Loads the configuration file path from an environment variable.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        public void UseConfigurationPathEnvironmentVariable(string variableName) =>
            context.UseConfigurationSources(
                () => [JsonFileConfigurationSource.FromPathEnvironmentVariable<TConfiguration>(variableName)]
            );

        /// <summary>
        /// Uses the provided configuration object.
        /// </summary>
        public void UseConfiguration(TConfiguration configuration) =>
            context.UseConfigurationSources(
                () => [DelegateConfigurationSource.Create("explicit", () => configuration)]
            );
    }
}
