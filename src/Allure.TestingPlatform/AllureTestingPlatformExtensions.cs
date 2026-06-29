using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Builder;

namespace Allure.TestingPlatform;

public static class AllureTestingPlatformExtensions
{
    extension (ITestApplicationBuilder builder)
    {
        public void AddAllure(Action<IStandaloneAllureRegistrationContext> configureAllure) =>
            AllureRegistrationFunctions.RegisterAllureTestingPlatform(
                builder,
                configureAllure,
                AllureTestingPlatformRegistrationMode.Standalone
            );

        public void AddAllure() =>
            AddAllure(builder, static (_) => {});
    }

    extension (IStandaloneAllureRegistrationContext registration)
    {
        public IStandaloneAllureRegistrationContext UseConfigurationFile<TConfiguration>(string file)
            where TConfiguration : AllureConfiguration, new() =>

            registration.UseConfiguration((serviceProvider) =>
            ConfigurationFunctions.ReadConfiguration<TConfiguration>(serviceProvider, file));

        public IStandaloneAllureRegistrationContext UseConfigurationFile(string file) =>
            registration.UseConfiguration((serviceProvider) =>
            ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(serviceProvider, file));

        public IStandaloneAllureRegistrationContext UseConfiguration<TConfiguration>(TConfiguration configuration)
            where TConfiguration : AllureConfiguration, new() =>

            registration.UseConfiguration((_) => configuration);

        public IStandaloneAllureRegistrationContext UseConfiguration(AllureConfiguration configuration) =>
            registration.UseConfiguration((serviceProvider) => configuration);

        public IStandaloneAllureRegistrationContext UseTypeFormatters(params IEnumerable<ITypeFormatter> formatters) =>
            registration.UseTypeFormatters(AllureRegistrationFunctions.ExplicitTypeFormatters(formatters));
    }
}
