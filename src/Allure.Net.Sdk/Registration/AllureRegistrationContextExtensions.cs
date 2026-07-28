using System;
using System.Text.Json;
using Allure.Sdk.Configuration;
using Allure.Sdk.Serialization;

namespace Allure.Sdk.Registration;

public static class AllureRegistrationContextExtensions
{
    extension<TConfiguration>(
        IAllureRuntimeRegistrationContext<TConfiguration> context
    )
        where TConfiguration : AllureConfiguration, new()
    {
        /// <summary>
        /// Load a configuration from the specified source.
        /// </summary>
        /// <param name="sourceFactory">A function that creates the configuration source.</param>
        public void UseConfigurationSource(Func<IAllureConfigurationSource<TConfiguration>> sourceFactory) =>
            context.UseConfigurationSources(() => [sourceFactory()]);

        /// <summary>
        /// Load configuration of a specific type from the specified JSON file.
        /// </summary>
        public void UseConfigurationFile(string path) =>
            context.UseConfigurationSources(
                () => [new JsonFileConfigurationSource<TConfiguration>(path)]
            );

        /// <summary>
        /// Load configuration from an environment variable.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        public void UseConfigurationPathEnvironmentVariable(string variableName) =>
            context.UseConfigurationSources(
                () => [JsonFileConfigurationSource.FromPathEnvironmentVariable<TConfiguration>(variableName)]
            );

        /// <summary>
        /// Use the provided configuration object.
        /// </summary>
        public void UseConfiguration(TConfiguration configuration) =>
            context.UseConfigurationSources(
                () => [DelegateConfigurationSource.Create("explicit", () => configuration)]
            );
    }
}
