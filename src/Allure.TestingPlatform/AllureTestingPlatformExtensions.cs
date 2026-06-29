using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;
using Microsoft.Testing.Platform.Builder;

namespace Allure.TestingPlatform;

public static class AllureTestingPlatformExtensions
{
    extension (ITestApplicationBuilder builder)
    {
        public void AddAllure(Action<IStandaloneRegistrationContext> configureAllure) =>
            AllureRegistrationFunctions.RegisterAllureTestingPlatform(
                builder,
                configureAllure,
                AllureTestingPlatformRegistrationMode.Standalone
            );

        public void AddAllure() =>
            AddAllure(builder, static (_) => {});
    }

    extension (IStandaloneRegistrationContext registration)
    {
        public IStandaloneRegistrationContext UseConfigurationFile<TConfiguration>(string file)
            where TConfiguration : AllureConfiguration, new() =>

            registration.UseConfiguration((serviceProvider) =>
            ConfigurationFunctions.ReadConfiguration<TConfiguration>(serviceProvider, file));

        public IStandaloneRegistrationContext UseConfigurationFile(string file) =>
            registration.UseConfiguration((serviceProvider) =>
            ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(serviceProvider, file));

        public IStandaloneRegistrationContext UseConfiguration<TConfiguration>(TConfiguration configuration)
            where TConfiguration : AllureConfiguration, new() =>

            registration.UseConfiguration((_) => configuration);

        public IStandaloneRegistrationContext UseConfiguration(AllureConfiguration configuration) =>
            registration.UseConfiguration((serviceProvider) => configuration);

        public IStandaloneRegistrationContext UseTypeFormatters(params IEnumerable<ITypeFormatter> formatters) =>
            registration.UseTypeFormatters(AllureRegistrationFunctions.ExplicitTypeFormatters(formatters));
    }
}
