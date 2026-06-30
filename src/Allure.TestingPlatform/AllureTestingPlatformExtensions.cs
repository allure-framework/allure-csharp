using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Builder;

namespace Allure.TestingPlatform;

/// <summary>
/// Provides registration helpers for the standalone Allure.TestingPlatform package.
/// </summary>
public static class AllureTestingPlatformExtensions
{
    extension (ITestApplicationBuilder builder)
    {
        /// <summary>
        /// Adds Allure.TestingPlatform to the test application and configures it.
        /// </summary>
        public void AddAllure(Action<IStandaloneAllureRegistrationContext> configureAllure) =>
            AllureTestingPlatformSdkExtensions.RegisterAllureTestingPlatform(
                builder,
                configureAllure,
                AllureTestingPlatformRegistrationMode.Standalone
            );

        /// <summary>
        /// Adds Allure.TestingPlatform to the test application with default settings.
        /// </summary>
        public void AddAllure() =>
            AddAllure(builder, static (_) => {});
    }

    extension (IStandaloneAllureRegistrationContext registration)
    {
        /// <summary>
        /// Load configuration read from the specified JSON file.
        /// </summary>
        public IStandaloneAllureRegistrationContext UseConfigurationFile(string file) =>
            registration.UseConfiguration((serviceProvider) =>
            ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(serviceProvider, file));

        /// <summary>
        /// Use the specified Allure configuration instance.
        /// </summary>
        public IStandaloneAllureRegistrationContext UseConfiguration(AllureConfiguration configuration) =>
            registration.UseConfiguration((serviceProvider) => configuration);

        /// <summary>
        /// Use the specified type formatters to serialize arguments into Allure parameter values.
        /// </summary>
        public IStandaloneAllureRegistrationContext UseTypeFormatters(params IEnumerable<ITypeFormatter> formatters) =>
            registration.UseTypeFormatters(AllureRegistrationDefaults.ExplicitTypeFormatters(formatters));
    }
}
