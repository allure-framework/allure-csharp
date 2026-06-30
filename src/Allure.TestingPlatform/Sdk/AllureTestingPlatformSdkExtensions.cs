using System;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Correlation;
using Microsoft.Testing.Platform.Builder;
using Allure.TestingPlatform.Internal.Registration;
using Microsoft.Testing.Platform.Extensions;
using Allure.TestingPlatform.Sdk.TestingPlatformExtensions;
using Microsoft.Testing.Platform.Services;
using Allure.Net.Commons.Configuration;
using Allure.TestingPlatform.Functions;

namespace Allure.TestingPlatform.Sdk;

/// <summary>
/// Provides registration helpers for SDK integrations built on Allure.TestingPlatform.
/// </summary>
public static class AllureTestingPlatformSdkExtensions
{
    extension (ITestApplicationBuilder builder)
    {
        internal IAllureTestingPlatformRuntimeReferenceRegistry RegisterAllureTestingPlatform(
            Action<AllureTestingPlatformRegistration> configureAllure,
            AllureTestingPlatformRegistrationMode registrationMode
        )
        {
            builder.CommandLine.AddProvider(() => new AllureCliOptionsProvider());

            var allureRegistration = new AllureTestingPlatformRegistration(registrationMode);
            configureAllure(allureRegistration);
            var frozenRegistration = allureRegistration.Prepare();
            var allureRuntimeReferences = frozenRegistration.RuntimeReferences;

            var factory =
                new CompositeExtensionFactory<AllureDataConsumer>((serviceProvider) =>
                    new AllureDataConsumer(
                        allureRuntimeReferences.GetRuntimeReference(serviceProvider)
                    )
                );

            builder.TestHostControllers.AddProcessLifetimeHandler((serviceProvider) =>
                new AllureTestingPlatformHostProcessWatchdog(
                    isEnabled: AllureCliOptionsProvider.GetWatchdogToggleValue(
                        serviceProvider.GetCommandLineOptions()
                    ) ?? frozenRegistration.HostProcessWatchdogEnabled,
                    runtimeController: frozenRegistration.CreateController(serviceProvider)
                )
            );

            builder.TestHost.AddTestHostApplicationLifetime((serviceProvider) =>
                new AllureTestingPlatformInProcessRuntimeController(
                    frozenRegistration.CreateController(serviceProvider)
                )
            );
            builder.TestHost.AddDataConsumer(factory);
            builder.TestHost.AddTestSessionLifetimeHandler(factory);

            return frozenRegistration.RuntimeReferences;
        }

        /// <summary>
        /// Adds Allure.TestingPlatform in embedded mode and configures it.
        /// </summary>
        public IAllureTestingPlatformRuntimeReferenceRegistry AddEmbeddedAllure(
            Action<IEmbeddedAllureRegistrationContext> configureAllure
        ) =>
            RegisterAllureTestingPlatform(
                builder,
                configureAllure,
                AllureTestingPlatformRegistrationMode.Embedded
            );

        /// <summary>
        /// Adds Allure.TestingPlatform in embedded mode with default settings.
        /// </summary>
        public IAllureTestingPlatformRuntimeReferenceRegistry AddEmbeddedAllure() =>
            AddEmbeddedAllure(builder, static (_) => {});
    }

    extension (IEmbeddedAllureRegistrationContext context)
    {
        /// <summary>
        /// Load configuration of a specific type from the specified JSON file.
        /// </summary>
        public IEmbeddedAllureRegistrationContext UseConfigurationFile<TConfiguration>(string file)
            where TConfiguration : AllureConfiguration, new()
        {
            context.UseConfiguration((serviceProvider) =>
                ConfigurationFunctions.ReadConfiguration<TConfiguration>(serviceProvider, file));
            return context;
        }

        /// <summary>
        /// Uses the specified configuration instance.
        /// </summary>
        public IEmbeddedAllureRegistrationContext UseConfiguration<TConfiguration>(TConfiguration configuration)
            where TConfiguration : AllureConfiguration, new()
        {
            context.UseConfiguration((_) => configuration);
            return context;
        }

        /// <summary>
        /// Correlates SDK messages by Microsoft Testing Platform session UID.
        /// </summary>
        public IEmbeddedAllureRegistrationContext UseMtpSessionCorrelation() =>
            context.UseCorrelation((_, _) => new TestingPlatformSessionUidCorrelationStrategy());

        /// <summary>
        /// Correlates SDK messages by <see cref="Microsoft.Testing.Platform.Extensions.Messages.TestMetadataProperty"/>
        /// with key <see cref="TestNodeMetadataCorrelationStrategy.MetadataKey"/>.
        /// </summary>
        public IEmbeddedAllureRegistrationContext UseTestNodeMetadataCorrelation() =>
            context.UseCorrelation((_, _) => new TestNodeMetadataCorrelationStrategy());
    }
}
