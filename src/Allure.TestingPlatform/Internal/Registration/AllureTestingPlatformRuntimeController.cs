using System;
using System.Collections.Immutable;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.TestingPlatformExtensions;
using Microsoft.Testing.Platform.Services;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformRuntimeController(
    AllureTestingPlatformRegistrationInput input,
    IServiceProvider serviceProvider,
    AllureTestingPlatformRuntimeReference runtimeReference
) :
    IAllureTestingPlatformRuntimeController
{
    IAllureTestingPlatformRuntimeReference IAllureTestingPlatformRuntimeController.RuntimeReference => runtimeReference;

    public AllureTestingPlatformRuntimeState Configure()
    {
        _ = runtimeReference.CurrentRuntime switch
        {
            SuppressedAllureTestingPlatformRuntime =>
                throw new InvalidOperationException(
                    "Cannot configure Allure.TestingPlatform: "
                        + "Allure.TestingPlatform is suppressed via the CLI"
                ),

            { Phase: not AllureTestingPlatformRuntimePhase.NotInitialized } =>
                throw new InvalidOperationException(
                    "Cannot configure Allure.TestingPlatform: "
                        + "Allure.TestingPlatform is already configured"
                ),

            var value => value,
        };

        var cliOptions = serviceProvider.GetCommandLineOptions();
        var allureToggle = AllureCliOptionsProvider.GetAllureToggleValue(cliOptions);
        if (allureToggle == false)
        {
            var suppressedRuntime = new SuppressedAllureTestingPlatformRuntime(input.Mode);
            runtimeReference.CurrentRuntime = suppressedRuntime;
            return suppressedRuntime;
        }

        var configuration = input.ConfigurationFactory(serviceProvider);
        var logger = input.LoggerFactory(serviceProvider, configuration);

        ConfiguredAllureTestingPlatformRuntime configuredRuntime =
            allureToggle is null && !input.IsSdkEnabled(serviceProvider, configuration)
                ? new DisabledAllureTestingPlatformRuntime(input.Mode, logger, configuration)
                : new EnabledAllureTestingPlatformRuntime(input.Mode, logger, configuration);

        runtimeReference.CurrentRuntime = configuredRuntime;
        input.OnConfigured?.Invoke(configuredRuntime);

        return configuredRuntime;
    }

    public AllureTestingPlatformRuntimeState Start()
    {
        var configuredRuntime = runtimeReference.CurrentRuntime switch
        {
            EnabledAllureTestingPlatformRuntime catp => catp,

            SuppressedAllureTestingPlatformRuntime =>
                throw new InvalidOperationException(
                    "Cannot start Allure.TestingPlatform runtime: "
                        + "the runtime is suppressed via the CLI."
                ),

            DisabledAllureTestingPlatformRuntime =>
                throw new InvalidOperationException(
                    "Cannot start Allure.TestingPlatform runtime: "
                        + "the runtime is disabled."
                ),

            LiveAllureTestingPlatformRuntime =>
                throw new InvalidOperationException(
                    "Cannot start Allure.TestingPlatform runtime: "
                        + "the runtime is already live."
                ),

            _ => throw new InvalidOperationException(
                "Cannot start Allure.TestingPlatform runtime: "
                    + "Allure.TestingPlatform must be configured first."
            ),
        };

        var configuration = configuredRuntime.Configuration;
        var logger = configuredRuntime.Logger;

        var writer = input.WriterFactory(serviceProvider, configuration);
        var typeFormatters = input.TypeFormattersFactory(serviceProvider, configuration)
            .ToImmutableDictionary();

        var liveRuntime = new LiveAllureTestingPlatformRuntime(
            Mode: input.Mode,
            Logger: logger,
            Configuration: configuration,
            CorrelationStrategy: input.CorrelationStrategyFactory(serviceProvider, configuration),
            Writer: writer,
            TypeFormatters: typeFormatters,
            Lifecycle: input.LifecycleFactory(
                serviceProvider,
                new(
                    Config: configuration,
                    Writer: writer,
                    TypeFormatters: typeFormatters
                )
            )
        );
        runtimeReference.CurrentRuntime = liveRuntime;
        input.OnLive?.Invoke(liveRuntime);
        return liveRuntime;
    }
}
