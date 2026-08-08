using System;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Correlation;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;
using Allure.TestingPlatform.Sdk.TestingPlatformExtensions;
using Microsoft.Testing.Platform.Services;
using Allure.TestingPlatform.Configuration;
using Allure.Sdk.Registration;
using Allure.Abstractions;
using Allure.TestingPlatform.Internal.Runtime;
using Allure.TestingPlatform.Registration;

namespace Allure.TestingPlatform.Sdk;

/// <summary>
/// Provides registration helpers for SDK integrations built on Allure.TestingPlatform.
/// </summary>
public static class AllureTestingPlatformSdkExtensions
{
    extension (ITestApplicationBuilder builder)
    {
        internal IReadOnlyLateBoundReference<TRuntime> RegisterAllureTestingPlatform<
            TConfiguration,
            TRuntimeRegistrationContext,
            TRuntimeHook,
            TEndpointRegistrationContext,
            TEndpointHook,
            TRuntimeIntegrationContext,
            TIntegrationSnapshot,
            TRuntime
        >(
            string runtimeName,
            Func<
                AllureRuntimeRegistrationSession<
                    TConfiguration,
                    TRuntimeIntegrationContext,
                    TRuntime
                >
            > sessionFactory,
            Action<TRuntimeIntegrationContext, IServiceProvider> registration
        )
            where TConfiguration : AllureTestingPlatformConfiguration, new()
            where TRuntimeRegistrationContext : IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
            where TRuntimeHook : IAllureTestingPlatformRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
            where TEndpointRegistrationContext : IAllureTestingPlatformEndpointRegistrationContext<TConfiguration, TRuntime>
            where TEndpointHook : IAllureTestingPlatformEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext, TRuntime>
            where TRuntimeIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContext<
                TConfiguration,
                TRuntimeRegistrationContext,
                TRuntimeHook,
                TEndpointRegistrationContext,
                TEndpointHook,
                TRuntime
            >
            where TIntegrationSnapshot : IAllureRuntimeIntegrationSnapshot<
                TConfiguration,
                TEndpointRegistrationContext,
                TEndpointHook,
                TRuntime
            >
            where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
        {
            var allureRuntimeBuilder = new AllureRuntimeBuilder<
                TConfiguration,
                TRuntimeRegistrationContext,
                TRuntimeHook,
                TEndpointRegistrationContext,
                TEndpointHook,
                TRuntimeIntegrationContext,
                TIntegrationSnapshot,
                TRuntime
            >(runtimeName, sessionFactory);

            IAllureRuntimeRegistrationPlan<TConfiguration, TRuntime>? registrationPlan = null;

            var allureRuntimeReference = allureRuntimeBuilder.RuntimeReference;

            builder.CommandLine.AddProvider(() => new AllureCliOptionsProvider());

            var factory =
                new CompositeExtensionFactory<AllureDataConsumer<TConfiguration, TRuntime>>((serviceProvider) =>
                    new AllureDataConsumer<TConfiguration, TRuntime>(allureRuntimeReference)
                );

            builder.TestHostControllers.AddProcessLifetimeHandler((serviceProvider) =>
                new AllureTestingPlatformHostProcessWatchdog<TConfiguration, TRuntime>(
                    registrationPlan ??= allureRuntimeBuilder.Prepare(
                        (ctx) => RegisterIntegration(ctx, serviceProvider)
                    )
                )
            );

            builder.TestHost.AddTestHostApplicationLifetime((serviceProvider) =>
                new AllureTestingPlatformInProcessRuntimeController<TConfiguration, TRuntime>(
                    registrationPlan ??= allureRuntimeBuilder.Prepare(
                        (ctx) => RegisterIntegration(ctx, serviceProvider)
                    )
                )
            );
            builder.TestHost.AddDataConsumer(factory);
            builder.TestHost.AddTestSessionLifetimeHandler(factory);

            return allureRuntimeReference;

            void RegisterIntegration(
                TRuntimeIntegrationContext context,
                IServiceProvider serviceProvider
            )
            {
                context.UseLogger(
                    (_) => serviceProvider
                        .GetLoggerFactory()
                        .CreateLogger("Allure")
                );

                context.UseMessageBus((_) => serviceProvider.GetMessageBus());

                if (serviceProvider.GetConfiguration()["platformOptions:resultDirectory"] is { } mtpResultsDir)
                {
                    context.TransformConfiguration(
                        (cfg) => cfg.WithPropertyIfUnset(
                            c => c.ResultsDirectory,
                            mtpResultsDir,
                            (c, v) => c with { ResultsDirectory = v }
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

                registration(context, serviceProvider);
            }
        }

        public IReadOnlyLateBoundReference<TRuntime> AddEmbeddedAllure<
            TConfiguration,
            TRuntimeRegistrationContext,
            TRuntimeHook,
            TEndpointRegistrationContext,
            TEndpointHook,
            TRuntimeIntegrationContext,
            TIntegrationSnapshot,
            TRuntime
        >(
            string runtimeName,
            Func<
                AllureRuntimeRegistrationSession<
                    TConfiguration,
                    TRuntimeIntegrationContext,
                    TRuntime
                >
            > sessionFactory,
            Action<TRuntimeIntegrationContext, IServiceProvider> runtimeRegistration,
            Action<
                IAllureInProcessEndpointIntegrationContext<
                    TConfiguration,
                    TEndpointRegistrationContext,
                    TEndpointHook,
                    TRuntime
                >,
                IServiceProvider,
                TRuntime
            > endpointRegistration
        )
            where TConfiguration : AllureTestingPlatformConfiguration, new()
            where TRuntimeRegistrationContext : IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
            where TRuntimeHook : IAllureTestingPlatformRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
            where TEndpointRegistrationContext : IAllureTestingPlatformEndpointRegistrationContext<TConfiguration, TRuntime>
            where TEndpointHook : IAllureTestingPlatformEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext, TRuntime>
            where TRuntimeIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContext<
                TConfiguration,
                TRuntimeRegistrationContext,
                TRuntimeHook,
                TEndpointRegistrationContext,
                TEndpointHook,
                TRuntime
            >
            where TIntegrationSnapshot : IAllureRuntimeIntegrationSnapshot<
                TConfiguration,
                TEndpointRegistrationContext,
                TEndpointHook,
                TRuntime
            >
            where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
        {
            IReadOnlyLateBoundReference<TRuntime>? runtimeReference = null;
            return runtimeReference = RegisterAllureTestingPlatform<
                TConfiguration,
                TRuntimeRegistrationContext,
                TRuntimeHook,
                TEndpointRegistrationContext,
                TEndpointHook,
                TRuntimeIntegrationContext,
                TIntegrationSnapshot,
                TRuntime
            >(builder, runtimeName, sessionFactory, (context, serviceProvider) =>
            {
                context.RegisterInProcessEndpoint(runtimeName, (runtime, endpointContext) =>
                {
                    endpointContext.UseCurrentScopePredicate((runtime) =>
                        runtime.ExecutionStateContext is { CurrentTestUid: not null } or { CurrentFixtureUid: not null});

                    endpointContext.UseGlobalScopePredicate((_) => runtimeReference?.IsBound == true);
                    endpointContext.SetAvailabilityPredicate((_) => runtimeReference?.IsBound == true);
                    endpointContext.UseOperations((runtime) =>
                    {
                        var asyncOperations = new AllureTestingPlatformAsyncOperations(runtime);
                        return new AllureInProcessOperations(
                            new AllureTestingPlatformSyncOperations(asyncOperations),
                            asyncOperations
                        );
                    });

                    endpointRegistration(endpointContext, serviceProvider, runtime);
                });

                runtimeRegistration(context, serviceProvider);
            });
        }

        public IReadOnlyLateBoundReference<TRuntime> AddEmbeddedAllure<
            TConfiguration,
            TRuntimeRegistrationContext,
            TRuntimeHook,
            TEndpointRegistrationContext,
            TEndpointHook,
            TRuntimeIntegrationContext,
            TIntegrationSnapshot,
            TRuntime
        >(
            string runtimeName,
            Func<
                AllureRuntimeRegistrationSession<
                    TConfiguration,
                    TRuntimeIntegrationContext,
                    TRuntime
                >
            > sessionFactory,
            Action<TRuntimeIntegrationContext, IServiceProvider> registration
        )
            where TConfiguration : AllureTestingPlatformConfiguration, new()
            where TRuntimeRegistrationContext : IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
            where TRuntimeHook : IAllureTestingPlatformRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
            where TEndpointRegistrationContext : IAllureTestingPlatformEndpointRegistrationContext<TConfiguration, TRuntime>
            where TEndpointHook : IAllureTestingPlatformEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext, TRuntime>
            where TRuntimeIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContext<
                TConfiguration,
                TRuntimeRegistrationContext,
                TRuntimeHook,
                TEndpointRegistrationContext,
                TEndpointHook,
                TRuntime
            >
            where TIntegrationSnapshot : IAllureRuntimeIntegrationSnapshot<
                TConfiguration,
                TEndpointRegistrationContext,
                TEndpointHook,
                TRuntime
            >
            where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
        =>
            AddEmbeddedAllure<
                TConfiguration,
                TRuntimeRegistrationContext,
                TRuntimeHook,
                TEndpointRegistrationContext,
                TEndpointHook,
                TRuntimeIntegrationContext,
                TIntegrationSnapshot,
                TRuntime
            >(builder, runtimeName, sessionFactory, registration, (_, _, _) => {});

        public IReadOnlyLateBoundReference<IAllureTestingPlatformRuntime<TConfiguration>> AddEmbeddedAllure<
            TConfiguration,
            TRuntimeRegistrationContext,
            TRuntimeHook,
            TEndpointRegistrationContext,
            TEndpointHook,
            TRuntimeIntegrationContext,
            TIntegrationSnapshot
        >(
            string runtimeName,
            Func<
                AllureRuntimeRegistrationSession<
                    TConfiguration,
                    TRuntimeIntegrationContext,
                    IAllureTestingPlatformRuntime<TConfiguration>
                >
            > sessionFactory,
            Action<TRuntimeIntegrationContext, IServiceProvider> runtimeRegistration,
            Action<
                IAllureInProcessEndpointIntegrationContext<
                    TConfiguration,
                    TEndpointRegistrationContext,
                    TEndpointHook,
                    IAllureTestingPlatformRuntime<TConfiguration>
                >,
                IServiceProvider,
                IAllureTestingPlatformRuntime<TConfiguration>
            > endpointRegistration
        )
            where TConfiguration : AllureTestingPlatformConfiguration, new()
            where TRuntimeRegistrationContext : IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
            where TRuntimeHook : IAllureTestingPlatformRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
            where TEndpointRegistrationContext : IAllureTestingPlatformEndpointRegistrationContext<TConfiguration>
            where TEndpointHook : IAllureTestingPlatformEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext>
            where TRuntimeIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContext<
                TConfiguration,
                TRuntimeRegistrationContext,
                TRuntimeHook,
                TEndpointRegistrationContext,
                TEndpointHook
            >
            where TIntegrationSnapshot : IAllureRuntimeIntegrationSnapshot<
                TConfiguration,
                TEndpointRegistrationContext,
                TEndpointHook,
                IAllureTestingPlatformRuntime<TConfiguration>
            >
        =>
            AddEmbeddedAllure<
                TConfiguration,
                TRuntimeRegistrationContext,
                TRuntimeHook,
                TEndpointRegistrationContext,
                TEndpointHook,
                TRuntimeIntegrationContext,
                TIntegrationSnapshot,
                IAllureTestingPlatformRuntime<TConfiguration>
            >(builder, runtimeName, sessionFactory, runtimeRegistration, endpointRegistration);

        public IReadOnlyLateBoundReference<IAllureTestingPlatformRuntime<TConfiguration>> AddEmbeddedAllure<
            TConfiguration,
            TRuntimeRegistrationContext,
            TRuntimeHook,
            TEndpointRegistrationContext,
            TEndpointHook,
            TRuntimeIntegrationContext,
            TIntegrationSnapshot
        >(
            string runtimeName,
            Func<
                AllureRuntimeRegistrationSession<
                    TConfiguration,
                    TRuntimeIntegrationContext,
                    IAllureTestingPlatformRuntime<TConfiguration>
                >
            > sessionFactory,
            Action<TRuntimeIntegrationContext, IServiceProvider> registration
        )
            where TConfiguration : AllureTestingPlatformConfiguration, new()
            where TRuntimeRegistrationContext : IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
            where TRuntimeHook : IAllureTestingPlatformRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
            where TEndpointRegistrationContext : IAllureTestingPlatformEndpointRegistrationContext<TConfiguration>
            where TEndpointHook : IAllureTestingPlatformEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext>
            where TRuntimeIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContext<
                TConfiguration,
                TRuntimeRegistrationContext,
                TRuntimeHook,
                TEndpointRegistrationContext,
                TEndpointHook
            >
            where TIntegrationSnapshot : IAllureRuntimeIntegrationSnapshot<
                TConfiguration,
                TEndpointRegistrationContext,
                TEndpointHook,
                IAllureTestingPlatformRuntime<TConfiguration>
            >
        =>
            AddEmbeddedAllure<
                TConfiguration,
                TRuntimeRegistrationContext,
                TRuntimeHook,
                TEndpointRegistrationContext,
                TEndpointHook,
                TRuntimeIntegrationContext,
                TIntegrationSnapshot,
                IAllureTestingPlatformRuntime<TConfiguration>
            >(builder, runtimeName, sessionFactory, registration);

        /// <summary>
        /// Adds Allure.TestingPlatform in embedded mode and configures it.
        /// </summary>
        public IReadOnlyLateBoundReference<IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>> AddEmbeddedAllure(
            string runtimeName,
            Action<IAllureTestingPlatformRuntimeIntegrationContext, IServiceProvider> runtimeRegistration,
            Action<
                IAllureInProcessEndpointIntegrationContext<
                    AllureTestingPlatformConfiguration,
                    IAllureTestingPlatformEndpointRegistrationContext,
                    IAllureTestingPlatformEndpointRegistrationHook,
                    IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
                >,
                IServiceProvider,
                IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
            > endpointRegistration
        ) =>
            AddEmbeddedAllure<
                AllureTestingPlatformConfiguration,
                IAllureTestingPlatformRuntimeRegistrationContext,
                IAllureTestingPlatformRuntimeRegistrationHook,
                IAllureTestingPlatformEndpointRegistrationContext,
                IAllureTestingPlatformEndpointRegistrationHook,
                IAllureTestingPlatformRuntimeIntegrationContext,
                IAllureRuntimeIntegrationSnapshot<
                    AllureTestingPlatformConfiguration,
                    IAllureTestingPlatformEndpointRegistrationContext,
                    IAllureTestingPlatformEndpointRegistrationHook,
                    IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
                >
            >(
                builder,
                runtimeName,
                () => new AllureTestingPlatformRuntimeRegistrationSession(),
                runtimeRegistration,
                endpointRegistration
            );

        /// <summary>
        /// Adds Allure.TestingPlatform in embedded mode and configures it.
        /// </summary>
        public IReadOnlyLateBoundReference<IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>> AddEmbeddedAllure(
            string runtimeName,
            Action<IAllureTestingPlatformRuntimeIntegrationContext, IServiceProvider> registration
        ) =>
            AddEmbeddedAllure(builder, runtimeName, registration, (_, _, _) => { });
    }

    extension<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    > (
        IAllureTestingPlatformRuntimeIntegrationContext<
            TConfiguration,
            TRuntimeRegistrationContext,
            TRuntimeHook,
            TEndpointRegistrationContext,
            TEndpointHook,
            TRuntime
        > context
    )
        where TConfiguration :
            AllureTestingPlatformConfiguration,
            new()
        where TRuntimeRegistrationContext :
            IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
        where TRuntimeHook :
            IAllureTestingPlatformRuntimeRegistrationHook<
                TConfiguration,
                TRuntimeRegistrationContext
            >
        where TEndpointRegistrationContext :
            IAllureTestingPlatformEndpointRegistrationContext<TConfiguration, TRuntime>
        where TEndpointHook :
            IAllureTestingPlatformEndpointRegistrationHook<
                TConfiguration,
                TEndpointRegistrationContext,
                TRuntime
            >
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
