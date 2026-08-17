using System;
using System.IO;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Correlation;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Services;
using Allure.TestingPlatform.Configuration;
using Allure.Sdk.Registration;
using Allure.Abstractions;
using Allure.TestingPlatform.Internal.Runtime;
using Allure.TestingPlatform.Internal.TestingPlatformExtensions;

namespace Allure.TestingPlatform.Sdk;

/// <summary>
/// Provides registration helpers for SDK integrations built on Allure.TestingPlatform.
/// </summary>
public static class AllureTestingPlatformSdkExtensions
{
    /// <summary>
    /// Provides embedded Allure runtime registration methods for a test application builder.
    /// </summary>
    /// <param name="builder">The test application builder.</param>
    extension (ITestApplicationBuilder builder)
    {
        internal IAllureTestingPlatformRuntimeHandle<TConfiguration, TRuntime> RegisterAllureTestingPlatform<
            TConfiguration,
            TRuntime,
            TIntegrationContext
        >(
            string runtimeName,
            Func<
                AllureRuntimeRegistrationSessionBase<
                    TConfiguration,
                    TRuntime,
                    TIntegrationContext
                >
            > sessionFactory,
            Action<
                TIntegrationContext,
                AllureTestingPlatformRuntimeCoordinator<
                    TConfiguration,
                    TRuntime,
                    TIntegrationContext
                >
            > registration
        )
            where TConfiguration : AllureTestingPlatformConfiguration, new()
            where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
            where TIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContextBase<
                TConfiguration,
                TRuntime
            >
        {
            var runtimeCoordinator = AllureTestingPlatformRuntimeCoordinator.Create(
                runtimeName,
                sessionFactory,
                RegisterIntegration
            );
            var runtimeControl =
                (IAllureTestingPlatformRuntimeControl<
                    AllureTestingPlatformConfiguration,
                    IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
                >)
                (IAllureTestingPlatformRuntimeControl<TConfiguration, TRuntime>)
                runtimeCoordinator;

            builder.CommandLine.AddProvider(() => new AllureCliOptionsProvider());

            var factory =
                new CompositeExtensionFactory<AllureDataConsumer>((serviceProvider) =>
                {
                    var binding = runtimeCoordinator.BindConsumer(serviceProvider);
                    return new AllureDataConsumer(runtimeControl, binding);
                });

            builder.TestHostControllers.AddProcessLifetimeHandler((serviceProvider) =>
            {
                runtimeCoordinator.BindController(serviceProvider);
                return new AllureTestingPlatformHostProcessWatchdog(runtimeControl);
            });

            builder.TestHost.AddDataConsumer(factory);
            builder.TestHost.AddTestSessionLifetimeHandler(factory);
            builder.TestHost.AddTestHostApplicationLifetime((serviceProvider) =>
            {
                runtimeCoordinator.BindTestHost(serviceProvider);
                return new AllureTestingPlatformRuntimeRegistrationOwner(runtimeControl);
            });

            return runtimeCoordinator;

            void RegisterIntegration(
                TIntegrationContext context,
                AllureTestingPlatformRuntimeCoordinator<
                    TConfiguration,
                    TRuntime,
                    TIntegrationContext
                > coordinator
            )
            {
                var serviceProvider = coordinator.ServiceProvider;

                context.UseLogger(
                    (_) => serviceProvider
                        .GetLoggerFactory()
                        .CreateLogger("Allure")
                );

                if (serviceProvider.GetConfiguration()["platformOptions:resultDirectory"] is { } mtpResultsDir)
                {
                    context.TransformConfiguration(
                        (cfg) => cfg.WithPropertyIfUnset(
                            c => c.ResultsDirectory,
                            mtpResultsDir,
                            (c, v) => c with { ResultsDirectory = Path.Combine(v, "allure-results") }
                        )
                    );
                }

                var options = serviceProvider.GetCommandLineOptions();
                if (AllureCliOptionsProvider.GetAllureToggleValue(options) is { } isAllureEnabled)
                {
                    context.TransformConfiguration(
                        (cfg) => cfg.WithProperty(
                            c => c.IsEnabled,
                            isAllureEnabled,
                            (c, v) => c with { IsEnabled = v }
                        )
                    );
                }
                if (AllureCliOptionsProvider.GetWatchdogToggleValue(options) is { } isWatchdogEnabled)
                {
                    context.TransformConfiguration(
                        (cfg) => cfg.WithProperty(
                            c => c.IsProcessWatchdogEnabled,
                            isWatchdogEnabled,
                            (c, v) => c with { IsProcessWatchdogEnabled = v }
                        )
                    );
                }
                if (AllureCliOptionsProvider.GetResultsDirectoryValue(options) is { } resultsDirectory)
                {
                    context.TransformConfiguration(
                        (cfg) => cfg.WithProperty(
                            c => c.ResultsDirectory,
                            resultsDirectory,
                            (c, v) => c with { ResultsDirectory = v }
                        )
                    );
                }

                registration(context, coordinator);
            }
        }

        /// <summary>
        /// Adds and configures an embedded Allure runtime and its in-process endpoint.
        /// </summary>
        /// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
        /// <typeparam name="TRuntime">The runtime type.</typeparam>
        /// <typeparam name="TIntegrationContext">The integration context type.</typeparam>
        /// <param name="runtimeName">The name used to identify the runtime and endpoint.</param>
        /// <param name="sessionFactory">A factory that creates a runtime registration session.</param>
        /// <param name="runtimeRegistration">A callback that configures the runtime.</param>
        /// <param name="endpointRegistration">A callback that configures the in-process endpoint.</param>
        /// <returns>A handle that provides access to the registered runtime.</returns>
        public IAllureTestingPlatformRuntimeHandle<TConfiguration, TRuntime> AddEmbeddedAllure<
            TConfiguration,
            TRuntime,
            TIntegrationContext
        >(
            string runtimeName,
            Func<
                AllureRuntimeRegistrationSessionBase<
                    TConfiguration,
                    TRuntime,
                    TIntegrationContext
                >
            > sessionFactory,
            Action<TIntegrationContext, IServiceProvider> runtimeRegistration,
            Action<
                IAllureInProcessEndpointIntegrationContext<TRuntime>,
                IServiceProvider,
                TRuntime
            > endpointRegistration
        )
            where TConfiguration : AllureTestingPlatformConfiguration, new()
            where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
            where TIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContextBase<
                TConfiguration,
                TRuntime
            >
        =>
            RegisterAllureTestingPlatform(
                builder,
                runtimeName,
                sessionFactory,
                (context, coordinator) =>
                {
                    context.RegisterInProcessEndpoint(runtimeName, (runtime, endpointContext) =>
                    {
                        endpointContext.UseCurrentScopePredicate(
                            (runtime) => coordinator.CanPublish && runtime.ExecutionStateContext is
                                { CurrentTestUid: not null }
                                    or { CurrentFixtureUid: not null}
                        );
                        endpointContext.UseGlobalScopePredicate((_) => coordinator.CanPublish);
                        endpointContext.SetAvailabilityPredicate(
                            (_) => runtime.Configuration.IsEnabled && coordinator.CanPublish
                        );
                        endpointContext.UseOperations((runtime) =>
                        {
                            var asyncOperations = new AllureTestingPlatformAsyncOperations(
                                (IAllureTestingPlatformRuntimeHandle<
                                    AllureTestingPlatformConfiguration,
                                    IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
                                >)
                                (IAllureTestingPlatformRuntimeHandle<TConfiguration, TRuntime>)
                                coordinator
                            );
                            return new AllureInProcessOperations(
                                new AllureTestingPlatformSyncOperations(asyncOperations),
                                asyncOperations
                            );
                        });

                        endpointRegistration(endpointContext, coordinator.ServiceProvider, runtime);
                    });

                    runtimeRegistration(context, coordinator.ServiceProvider);
                }
            );

        /// <summary>
        /// Adds and configures an embedded Allure runtime.
        /// </summary>
        /// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
        /// <typeparam name="TRuntime">The runtime type.</typeparam>
        /// <typeparam name="TIntegrationContext">The integration context type.</typeparam>
        /// <param name="runtimeName">The name used to identify the runtime and endpoint.</param>
        /// <param name="sessionFactory">A factory that creates a runtime registration session.</param>
        /// <param name="registration">A callback that configures the runtime.</param>
        /// <returns>A handle that provides access to the registered runtime.</returns>
        public IAllureTestingPlatformRuntimeHandle<TConfiguration, TRuntime> AddEmbeddedAllure<
            TConfiguration,
            TRuntime,
            TIntegrationContext
        >(
            string runtimeName,
            Func<
                AllureRuntimeRegistrationSessionBase<
                    TConfiguration,
                    TRuntime,
                    TIntegrationContext
                >
            > sessionFactory,
            Action<TIntegrationContext, IServiceProvider> registration
        )
            where TConfiguration : AllureTestingPlatformConfiguration, new()
            where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
            where TIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContextBase<
                TConfiguration,
                TRuntime
            >
        =>
            AddEmbeddedAllure(builder, runtimeName, sessionFactory, registration, (_, _, _) => { });

        /// <summary>
        /// Adds and configures an embedded Allure runtime and its in-process endpoint using the
        /// default integration context.
        /// </summary>
        /// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
        /// <typeparam name="TRuntime">The runtime type.</typeparam>
        /// <param name="runtimeName">The name used to identify the runtime and endpoint.</param>
        /// <param name="sessionFactory">A factory that creates a runtime registration session.</param>
        /// <param name="runtimeRegistration">A callback that configures the runtime.</param>
        /// <param name="endpointRegistration">A callback that configures the in-process endpoint.</param>
        /// <returns>A handle that provides access to the registered runtime.</returns>
        public IAllureTestingPlatformRuntimeHandle<TConfiguration, TRuntime> AddEmbeddedAllure<
            TConfiguration,
            TRuntime
        >(
            string runtimeName,
            Func<
                AllureRuntimeRegistrationSessionBase<
                    TConfiguration,
                    TRuntime,
                    IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration, TRuntime>
                >
            > sessionFactory,
            Action<
                IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration, TRuntime>,
                IServiceProvider
            > runtimeRegistration,
            Action<
                IAllureInProcessEndpointIntegrationContext<TRuntime>,
                IServiceProvider,
                TRuntime
            > endpointRegistration
        )
            where TConfiguration : AllureTestingPlatformConfiguration, new()
            where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
        =>
            AddEmbeddedAllure<
                TConfiguration,
                TRuntime,
                IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration, TRuntime>
            >(
                builder,
                runtimeName,
                sessionFactory,
                runtimeRegistration,
                endpointRegistration
            );

        /// <summary>
        /// Adds and configures an embedded Allure runtime using the default integration context.
        /// </summary>
        /// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
        /// <typeparam name="TRuntime">The runtime type.</typeparam>
        /// <param name="runtimeName">The name used to identify the runtime and endpoint.</param>
        /// <param name="sessionFactory">A factory that creates a runtime registration session.</param>
        /// <param name="registration">A callback that configures the runtime.</param>
        /// <returns>A handle that provides access to the registered runtime.</returns>
        public IAllureTestingPlatformRuntimeHandle<TConfiguration, TRuntime> AddEmbeddedAllure<
            TConfiguration,
            TRuntime
        >(
            string runtimeName,
            Func<
                AllureRuntimeRegistrationSessionBase<
                    TConfiguration,
                    TRuntime,
                    IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration, TRuntime>
                >
            > sessionFactory,
            Action<
                IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration, TRuntime>,
                IServiceProvider
            > registration
        )
            where TConfiguration : AllureTestingPlatformConfiguration, new()
            where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
        =>
            AddEmbeddedAllure(
                builder,
                runtimeName,
                sessionFactory,
                registration,
                (_, _, _) => { }
            );

        /// <summary>
        /// Adds and configures the default embedded Allure runtime for a specific configuration
        /// type and configures its in-process endpoint.
        /// </summary>
        /// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
        /// <param name="runtimeName">The name used to identify the runtime and endpoint.</param>
        /// <param name="runtimeRegistration">A callback that configures the runtime.</param>
        /// <param name="endpointRegistration">A callback that configures the in-process endpoint.</param>
        /// <returns>A handle that provides access to the registered runtime.</returns>
        public IAllureTestingPlatformRuntimeHandle<
            TConfiguration,
            IAllureTestingPlatformRuntime<TConfiguration>
        > AddEmbeddedAllure<TConfiguration>(
            string runtimeName,
            Action<
                IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration>,
                IServiceProvider
            > runtimeRegistration,
            Action<
                IAllureInProcessEndpointIntegrationContext<IAllureTestingPlatformRuntime<TConfiguration>>,
                IServiceProvider,
                IAllureTestingPlatformRuntime<TConfiguration>
            > endpointRegistration
        )
            where TConfiguration : AllureTestingPlatformConfiguration, new()
        =>
            AddEmbeddedAllure(
                builder,
                runtimeName,
                () => new AllureTestingPlatformRuntimeRegistrationSession<TConfiguration>(),
                runtimeRegistration,
                endpointRegistration
            );

        /// <summary>
        /// Adds and configures the default embedded Allure runtime for a specific configuration type.
        /// </summary>
        /// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
        /// <param name="runtimeName">The name used to identify the runtime and endpoint.</param>
        /// <param name="registration">A callback that configures the runtime.</param>
        /// <returns>A handle that provides access to the registered runtime.</returns>
        public IAllureTestingPlatformRuntimeHandle<
            TConfiguration,
            IAllureTestingPlatformRuntime<TConfiguration>
        > AddEmbeddedAllure<TConfiguration>(
            string runtimeName,
            Action<
                IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration>,
                IServiceProvider
            > registration
        )
            where TConfiguration : AllureTestingPlatformConfiguration, new()
        =>
            AddEmbeddedAllure(
                builder,
                runtimeName,
                registration,
                (_, _, _) => { }
            );

        /// <summary>
        /// Adds and configures the default embedded Allure runtime and its in-process endpoint.
        /// </summary>
        /// <param name="runtimeName">The name used to identify the runtime and endpoint.</param>
        /// <param name="runtimeRegistration">A callback that configures the runtime.</param>
        /// <param name="endpointRegistration">A callback that configures the in-process endpoint.</param>
        /// <returns>A handle that provides access to the registered runtime.</returns>
        public IAllureTestingPlatformRuntimeHandle<
            AllureTestingPlatformConfiguration,
            IAllureTestingPlatformRuntime
        > AddEmbeddedAllure(
            string runtimeName,
            Action<
                IAllureTestingPlatformRuntimeIntegrationContext,
                IServiceProvider
            > runtimeRegistration,
            Action<
                IAllureInProcessEndpointIntegrationContext<IAllureTestingPlatformRuntime>,
                IServiceProvider,
                IAllureTestingPlatformRuntime
            > endpointRegistration
        ) =>
            AddEmbeddedAllure(
                builder,
                runtimeName,
                () => new AllureTestingPlatformRuntimeRegistrationSession(),
                runtimeRegistration,
                endpointRegistration
            );

        /// <summary>
        /// Adds and configures the default embedded Allure runtime.
        /// </summary>
        /// <param name="runtimeName">The name used to identify the runtime and endpoint.</param>
        /// <param name="registration">A callback that configures the runtime.</param>
        /// <returns>A handle that provides access to the registered runtime.</returns>
        public IAllureTestingPlatformRuntimeHandle<
            AllureTestingPlatformConfiguration,
            IAllureTestingPlatformRuntime
        > AddEmbeddedAllure(
            string runtimeName,
            Action<
                IAllureTestingPlatformRuntimeIntegrationContext,
                IServiceProvider
            > registration
        ) =>
            AddEmbeddedAllure(
                builder,
                runtimeName,
                registration,
                (_, _, _) => { }
            );
    }

    /// <summary>
    /// Provides correlation-strategy configuration methods for an Allure Microsoft Testing
    /// Platform integration context.
    /// </summary>
    /// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
    /// <typeparam name="TRuntime">The runtime type.</typeparam>
    /// <param name="context">The runtime integration context.</param>
    extension<TConfiguration, TRuntime> (
        IAllureTestingPlatformRuntimeIntegrationContextBase<
            TConfiguration,
            TRuntime
        > context
    )
        where TConfiguration : AllureTestingPlatformConfiguration, new()
        where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
    {
        /// <summary>
        /// Correlates SDK messages by Microsoft Testing Platform session UID.
        /// </summary>
        public void UseTestingPlatformSessionCorrelation() =>
            context.UseCorrelationStrategy((_) => new TestingPlatformSessionUidCorrelationStrategy());

        /// <summary>
        /// Correlates SDK messages by <see cref="Microsoft.Testing.Platform.Extensions.Messages.TestMetadataProperty"/>
        /// with key <see cref="TestNodeMetadataCorrelationStrategy.MetadataKey"/>.
        /// </summary>
        public void UseTestNodeMetadataCorrelation() =>
            context.UseCorrelationStrategy((_) => new TestNodeMetadataCorrelationStrategy());
    }
}
